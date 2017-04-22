using System;
using System.IO;
using System.Net;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ClientLib;
using ClientLib.Common;
using ClientLib.Core;
using ClientLib.Utilities;
using Easi365UI.Entities;
using Easi365UI.Service;
using Easi365UI.Windows.Controls;
using Microsoft.SharePoint.Client;
using System.Windows.Media.Imaging;
using IO = System.IO;
using ClientLib.Services;

namespace Easi365UI
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : EasiWindow
    {
        private UserHelper userHelper { get; set; }

        SynchronizationContext sc;
        CancellationTokenSource cts = null;
        Task logintask = null;
        SymmetricMethod _sm { get; set; }

        public Login()
        {
            InitializeComponent();

            this.MouseLeftButtonDown += Login_MouseLeftButtonDown;
        }

        void Login_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

       

        //登录窗体加载事件
        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {

            var allowsTransparency = this.AllowsTransparency;
            
            userHelper = new UserHelper(App.databasePath);

            _sm = new SymmetricMethod();
            UserInfo user = userHelper.GetLastLoginUserInfo();
            if (user != null)
            {
                txtUserName.Text = user.LoginName;
                if (user.IsRememberPwd)
                {
                    //解密密码
                    txtPwd.Password = _sm.Decrypto(user.PassWord);
                    ckbRememberPwd.IsChecked = true;
                }
                if (user.IsAutoLogin) ckbAutoLogin.IsChecked = true;
            }
        }

        // 任务中执行的方法
        private void ServerLogin(string name, string password, CancellationToken ct)
        {
            try
            {
                
                App.spCredentials = GetCredential(name, password);

                if (!ct.IsCancellationRequested)
                {
                    SyncManager sm = new SyncManager(
                        CoreManager.ConfigManager.Settings.ServerUrl,
                        CoreManager.ConfigManager.Settings.DbPath + Path.DirectorySeparatorChar + "EasiDB.mdb",
                        App.spCredentials);
                    sm.Init();

                    CoreManager.TaskManager.Server = sm.PersonalServer;
                    CoreManager.ConfigManager.Settings.CurrentUserName = name;//登录帐号xxxx@xxx.xxx.xxx
                    CoreManager.ConfigManager.Settings.PassWord = password;//登录密码

                    //ID
                    //var id = OrgService.GetStaffIdByLoginName(sm.CompanyServerSide.ClientCtx, CoreManager.ConfigManager.Settings.CurrentUserName);
                    //if (id != -1) CoreManager.ConfigManager.Settings.ID = id.ToString();
                    
                    // 使用同步上下文的Post方法把更新UI的方法让主线程执行
                    sc.Post(state =>
                    {
                        App.CurrentApp.UploadingFileDetailPage.SetLocalDB(sm.DB);

                        bool remenberPwd = false, autoLogin = false;
                        if (ckbRememberPwd.IsChecked == false) password = "";
                        else remenberPwd = true;
                        if (ckbAutoLogin.IsChecked == true) autoLogin = true;
                        // 保存用户信息
                        userHelper.UserInfoSetting(name, password, remenberPwd, autoLogin);

                       // CoreManager.IsAppStarted = true;

                        this.loading.Visibility = System.Windows.Visibility.Collapsed;

                        App.Easi365MainWindow = new MaxWindow(sm);
                        App.Easi365MainWindow.Show();

                        this.Close();
                    }, null);
                }
            }
            catch (WebException webEx)
            {
                sc.Post(state => 
                {
                    Logging.Add("登录错误.", webEx);
                    this.loading.Visibility = System.Windows.Visibility.Collapsed;
                    MessageBox.Show(this, "无法连接到远程服务器.");
                },null);
            }
            catch (Exception ex)
            {
                sc.Post(state =>
                {
                    Logging.Add("登录错误.", ex);
                    this.loading.Visibility = System.Windows.Visibility.Collapsed;
                    MessageBox.Show(this, "用户名或密码错误.");
                }, null);
            }
        }

        private static ICredentials GetCredential(string name, string password)
        {
            SecureString pwdSStr = new SecureString();
            foreach (char c in password) pwdSStr.AppendChar(c);

            CoreManager.ServerMode mode = (CoreManager.ServerMode)Enum.Parse(typeof(CoreManager.ServerMode), CoreManager.ConfigManager.Settings.ServerMode, true);
            switch (mode)
            {
                case CoreManager.ServerMode.Office365:
                    return new SharePointOnlineCredentials(name, pwdSStr);
                case CoreManager.ServerMode.Local:
                    return new NetworkCredential(Common.GetLoginNameWithoutDomain(name), pwdSStr); 
                default:
                    return new SharePointOnlineCredentials(name, pwdSStr); 
            }
        }

        //登录
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                this.loading.Visibility = System.Windows.Visibility.Visible;
                // 捕捉调用线程的同步上下文派生对象
                sc = SynchronizationContext.Current;
                cts = new CancellationTokenSource();

                string loginName = txtUserName.Text;
                string passWord = txtPwd.Password;

                logintask = new Task(() => ServerLogin(loginName, passWord, cts.Token), cts.Token);
                logintask.Start();
            }
            catch (Exception ex)
            {
                lblMsg.Content = "用户名或密码错误!";
            }
            finally { }
        }

        private string SplitUserNameForUrl(string username)
        {
            var parts = username.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            //parts[1] = parts[1].Replace('.', '_');
            return string.Join("_", parts[0], parts[1].Replace('.', '_'));
        }

        //登录窗体设置按钮事件
        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            SystemSettings settingsWindow = new SystemSettings();
            settingsWindow.ShowDialog();
        }
        
        //最小化登录窗体
        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        //关闭登录窗体
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void txtPwd_GotFocus(object sender, RoutedEventArgs e)
        {
            this.txtPwd.SelectAll();
        }

        private void txtUserName_LostFocus(object sender, RoutedEventArgs e)
        {
            this.txtPwd.Password = "";
            this.ckbRememberPwd.IsChecked = false;
            this.ckbAutoLogin.IsChecked = false;

            var loginName = txtUserName.Text.Trim();
            if (userHelper.IsUserInfoExists(loginName))
            {
                var user = userHelper.GetUserByLoginName(loginName);
                if (user != null)
                {
                    txtUserName.Text = user.LoginName;
                    if (user.IsRememberPwd)
                    {
                        //解密密码
                        txtPwd.Password = _sm.Decrypto(user.PassWord);
                        ckbRememberPwd.IsChecked = true;
                    }
                    if (user.IsAutoLogin) ckbAutoLogin.IsChecked = true;
                }
            }
        }
    }
}
