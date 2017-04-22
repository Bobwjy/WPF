using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using ClientLib;
using ClientLib.Core;
using ClientLib.Entities;
using ClientLib.Services;
using Easi365UI.Lync;
using Easi365UI.Models;
using Easi365UI.Models.SkyDrive;
using Easi365UI.Windows;
using Easi365UI.Windows.Controls;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Win32;
using SP = Microsoft.SharePoint.Client;

namespace Easi365UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : EasiWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            //设置未读邮件数
            //this.EmailTabItem.Header = App._unreadMailCount;
        }

        public MainWindow(SyncManager sm)
            : this()
        {
            _sm = sm;

            ListViewItems = new ObservableCollection<IEntryModel>();
            CompanyListViewItems = new ObservableCollection<IEntryModel>();

            LyncViewModel = new Models.LyncViewModel();
            try
            {
                sc = SynchronizationContext.Current;
                cts = new CancellationTokenSource();

                //初始化路径
                SkyDriveProfile profile = new SkyDriveProfile(_sm.PersonalServer, sc, cts);
                SkyDriveProfile companyProfile = new SkyDriveProfile(_sm.CompanyServerSide, sc, cts);

                //profile.AggregateExceptionCatched += new EventHandler<SkyDriveProfile.AggregateExceptionArgs>(HandleAggregateExceptions);
                _rootDir = SkyDriveItemModel.DefaultSkyDriveItemModel(profile, _sm.PersonalServer.RootFolderServerRelativeUrl);
                _companyRootDir = SkyDriveItemModel.DefaultSkyDriveItemModel(companyProfile, _sm.CompanyServerSide.RootFolderServerRelativeUrl);

                CurrentDirectory = _rootDir;
                CompanyCurrentDirectory = _companyRootDir;

                this.DataContext = this;

                CoreManager.ServerMode mode = (CoreManager.ServerMode)Enum.Parse(typeof(CoreManager.ServerMode), CoreManager.ConfigManager.Settings.ServerMode, true);
                if (mode == CoreManager.ServerMode.Office365)
                {
                    // ExChange认证初始化EmailService，获取未读邮件数
                    Task initES = new Task(() =>
                    {
                        var es = new EmailService(CoreManager.ConfigManager.Settings.CurrentUserName, CoreManager.ConfigManager.Settings.PassWord);
                        var unReadCount = es.GetUnreadMailCount();
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            App._unreadMailCount = unReadCount;
                            App._es = es;

                            //设置未读邮件数
                            this.EmailTabItem.Header = App._unreadMailCount;
                            EmailTabItem.IsEnabled = true;
                        }));
                    });
                    initES.Start();
                }
            }
            catch
            { }
        }

        #region 属性
        SyncManager _sm = null;
        IEntryModel _rootDir;
        IEntryModel _companyRootDir;
        CancellationTokenSource cts = null; //取消操作的标记 上传或下载文件时使用
        SynchronizationContext sc; //UI线程的上下文
        ObservableCollection<Department> _orgList = null;
        List<Email> _emailList = null;
        EmailService _es = null;

        Stack<IEntryModel> _backwardNavigations = new Stack<IEntryModel>();

        //Stack<IEntryModel> _backwardNavigations = new Stack<IEntryModel>();
        Stack<IEntryModel> _forwardNavigations = new Stack<IEntryModel>();

        public ObservableCollection<IEntryModel> ListViewItems { get; set; }
        public ObservableCollection<IEntryModel> CompanyListViewItems { get; set; }

        public LyncViewModel LyncViewModel { get; set; }

        //剪切板内容前缀
        private const string ClipboardTextPrefix = "Easi365Disk|";

        //当前目录
        IEntryModel _currentDir;
        public IEntryModel CurrentDirectory
        {
            get
            {
                return this._currentDir;
            }
            set
            {
                SetCurrentFolder(value);
            }
        }

        IEntryModel _companyCurrentDir;
        public IEntryModel CompanyCurrentDirectory
        {
            get
            {
                return this._companyCurrentDir;
            }
            set
            {
                SetCompanyCurrentFolder(value);
            }
        }

        public void SetCurrentFolder(IEntryModel em)
        {
            _currentDir = em;
            _currentDir.Profile.ListAsync(CurrentDirectory, ListViewItems);
        }

        public void SetCompanyCurrentFolder(IEntryModel em)
        {
            _companyCurrentDir = em;
            _companyCurrentDir.Profile.ListAsync(CompanyCurrentDirectory, CompanyListViewItems);
        }

        #endregion

        #region 事件
        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //this.userName.Text = App.web.CurrentUser.Title;
            //this.jobTitle.Text = App.web.CurrentUser.Context.ApplicationName;

            //设置控件高度
            InitControlHeight();

            //设置窗口浮动位置
            WindowFloatLocate();

            //初始化Lync
            InitLyncClient();
        }

        //设置控件高度
        private void InitControlHeight()
        {
            double cWidth = this.Width;             //当前面板宽度
            double cHeight = this.Height;           //当前面板高度

            persSpaceGrid.Height = cHeight - toolCanvas.Height - 138;
            FileListView.Height = persSpaceGrid.Height - 30;
            listGroup.Height = persSpaceGrid.Height - 58;
            listSearchContract.Height = persSpaceGrid.Height - 58;
            pnlGroupList.Height = persSpaceGrid.Height - 58;
            pnlSearchList.Height = persSpaceGrid.Height - 58;
            commonSpaceGrid.Height = cHeight - toolCanvas.Height - 138 - 40;
            orgGrid.Height = cHeight - toolCanvas.Height - 138 - 40;
            tasksGrid.Height = cHeight - toolCanvas.Height - 138 - 40;
            emailsGrid.Height = cHeight - toolCanvas.Height - 138 - 40;
        }

        /// <summary>
        /// 最小化窗体
        /// </summary>
        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            //close lync process
            Common.CloseLyncProcess();

            //关闭日志文件
            Logging.Exit();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 重写事件
        /// </summary>
        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                base.OnStateChanged(e);
            }
        }

        /// <summary>
        /// 计算窗体大小
        /// </summary>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double maxWidth = 550.0;                //主面板最大宽度
            //double minWidth = 290.0;              //主面板最小宽度
            double maxHeight = App.screenY * 0.95;  //主面板最小高度
            double minHeight = 550.0;               //主面板最大高度
            double cWidth = this.Width;             //当前面板宽度
            double cHeight = this.Height;           //当前面板高度

            if (cWidth > maxWidth) this.Width = maxWidth;
            if (cHeight < minHeight) this.Height = minHeight;
            if (cHeight > maxHeight) this.Height = maxHeight;

            cWidth = this.Width;
            cHeight = this.Height;

            MainGrid.Width = cWidth - 10;
            toolCanvas.Width = cWidth - 12;

            persSpaceGrid.Height = cHeight - toolCanvas.Height - 138;
            FileListView.Height = persSpaceGrid.Height - 30;
            listGroup.Height = persSpaceGrid.Height - 58;
            listSearchContract.Height = persSpaceGrid.Height - 58;
            pnlGroupList.Height = persSpaceGrid.Height - 58;
            pnlSearchList.Height = persSpaceGrid.Height - 58;
            commonSpaceGrid.Height = cHeight - toolCanvas.Height - 138 - 40;
            orgGrid.Height = cHeight - toolCanvas.Height - 138 - 40;
            tasksGrid.Height = cHeight - toolCanvas.Height - 138 - 40;
            emailsGrid.Height = cHeight - toolCanvas.Height - 138 - 40;
        }

        private void FileListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView list = (ListView)sender;
            var selectedItem = list.SelectedItem as SkyDriveItemModel;
            if (selectedItem != null)
            {
                if (selectedItem.IsDirectory)
                {
                    _backwardNavigations.Push(CurrentDirectory);
                    CurrentDirectory = selectedItem;
                }
                else
                {
                    try
                    {
                        bool needDownload = _sm.PersonalServer.CheckIfFileNeedDownload(selectedItem.ServerItem,
                            () =>
                            {
                                DialogWindow dialogWindow = new DialogWindow();
                                dialogWindow.TipText = "服务器的文件版本比较新，是否下载文件？";
                                var dlgResut = dialogWindow.ShowDialog();

                                return dlgResut.Value;
                            });

                        if (needDownload)
                            selectedItem.DownloadState.IsDownloading = true;

                        Thread t = new Thread(() =>
                        {
                            string localFile = _sm.PersonalServer.DownloadFile(selectedItem.ServerItem, needDownload, () =>
                            {
                                this.Dispatcher.Invoke(new Action(() =>
                                {
                                    selectedItem.DownloadState.IsDownloading = false;
                                }));
                            });

                            if (!string.IsNullOrEmpty(localFile))
                            {
                                System.Diagnostics.Process p = new System.Diagnostics.Process();
                                p.StartInfo.FileName = localFile;
                                p.Start();
                            }
                        });
                        t.IsBackground = true;
                        t.Start();
                    }
                    catch (Exception ex)
                    {
                        Logging.Add("下载文件出错.", ex);
                    }
                }
            }

        }

        /// <summary>
        /// TabItem改变事件
        /// </summary>
        private void EasiTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem ti = (TabItem)this.EasiTab.SelectedItem;
            string tiName = ti.Name;

            if (tiName == PersSpaceTabItem.Name) { }
            if (tiName == CommonSpaceTabItem.Name) { }
            if (tiName == OrgTabItem.Name) { }
            if (tiName == TaskTabItem.Name) { }
            if (tiName == EmailTabItem.Name) LoadEmails();
            //if (tiName == LyncTabItem.Name) InitLyncClient();
        }

        private void NewFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            NewFolder();
        }

        private void menuNewFolder_Click(object sender, RoutedEventArgs e)
        {
            NewFolder();
        }


        #endregion

        #region 功能方法
        /// <summary>
        /// 窗口浮动定位
        /// </summary>
        private void WindowFloatLocate()
        {
            double Left = App.screenX - (this.Width + 60);
            double Top = App.screenY - this.Height;
            if (Top <= 0 || Top <= 100) Top = 0;
            else if (Top <= 200) Top = Top / 3;
            else { Top = 80; }

            this.Left = Left;
            this.Top = Top;
        }

        /// <summary>
        /// 加载组织架构
        /// </summary>
        private void LoadOrgStructure()
        {
            //if (_orgList == null)
            //{
            //    this.OrgLoadingUC.Visibility = System.Windows.Visibility.Visible;
            //    Task task = new Task(() =>
            //    {
            //        OrgService os = new OrgService(CoreManager.ConfigManager.Settings.ServerUrl, App.spCredentials);
            //        os.Init();

            //        var results = os.GetDeptList();
            //        this.Dispatcher.Invoke(() =>
            //        {
            //            _orgList = results;
            //            orgTreeView.ItemsSource = _orgList;
            //            this.OrgLoadingUC.Visibility = System.Windows.Visibility.Collapsed;
            //        });
            //    });
            //    task.Start();
            //}
        }

        /// <summary>
        /// 加载邮件
        /// </summary>
        private void LoadEmails()
        {
            if (_emailList == null)
            {
                this.EmailLoadingUC.Visibility = System.Windows.Visibility.Visible;
                Task task = new Task(() =>
                {
                    var results = App._es.GetUnreadMailFromInbox();
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        _emailList = results;
                        EmailListView.ItemsSource = _emailList;
                        this.EmailLoadingUC.Visibility = System.Windows.Visibility.Collapsed;
                    }));
                });
                task.Start();
            }
        }
        #endregion

        #region Lync

        LyncClient _lyncClient;
        Dispatcher _dispatcher;

        #region 登录

        void InitLyncClient()
        {
            CoreManager.ServerMode mode = (CoreManager.ServerMode)Enum.Parse(typeof(CoreManager.ServerMode), CoreManager.ConfigManager.Settings.ServerMode, true);
            if (mode != CoreManager.ServerMode.Office365)
            {
                this.lblMessage.Text = "暂不支持本地模式登录！";
                return;
            }

            _dispatcher = Dispatcher.CurrentDispatcher;

            Task task = new Task(() =>
            {
                try
                {
                    //开启 UI Supperession 模式
                    var startKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Office\15.0\Lync", true);
                    if (startKey == null)
                    {
                        _dispatcher.Invoke(new Action(delegate()
                        {
                            this.lblMessage.Text = "未检测到 Lync 2013 客户端！";
                        }), null);
                        return;
                    }
                    startKey.SetValue("UISuppressionMode", 1, RegistryValueKind.DWord);

                    _lyncClient = LyncClient.GetClient();

                    if (_lyncClient.State == ClientState.Uninitialized)
                    {
                        _lyncClient.BeginInitialize(InitializeCallback, null);
                    }
                    else if (_lyncClient.State == ClientState.SignedIn)
                    {
                        Common.SignUser = new User(
                            CoreManager.ConfigManager.Settings.CurrentUserName,
                            CoreManager.ConfigManager.Settings.CurrentUserName,
                            CoreManager.ConfigManager.Settings.PassWord);

                        LoadContract();
                        return;
                    }
                    else if (_lyncClient.State == ClientState.SignedOut)
                    {
                        string uri = CoreManager.ConfigManager.Settings.CurrentUserName;
                        string username = CoreManager.ConfigManager.Settings.CurrentUserName;
                        string password = CoreManager.ConfigManager.Settings.PassWord;
                        Common.SignUser = new User(uri, username, password);

                        _lyncClient.BeginSignIn(uri, username, password, SignInCallback, null);

                        _dispatcher.Invoke(new Action(delegate()
                        {
                            this.lblMessage.Text = "正在登录到Lync，请稍候……";
                        }), null);
                    }

                    //初始化
                    _lyncClient.StateChanged += lyncClient_StateChanged;
                    _lyncClient.SignInDelayed += lyncClient_SignInDelayed;
                    _lyncClient.CredentialRequested += lyncClient_CredentialRequested;
                }
                catch (LyncClientException lce)
                {
                    _dispatcher.Invoke(new Action(delegate()
                    {
                        this.lblMessage.Text = "初始化失败！";
                    }), null);
                    Logging.Add("Lync Exception - InitLyncClient", lce);
                }
                catch (Exception ex)
                {
                    _dispatcher.Invoke(new Action(delegate()
                    {
                        this.lblMessage.Text = "初始化失败！";
                    }), null);
                    Logging.Add("Lync Exception - System Exception - InitLyncClient", ex);
                }
            });
            task.Start();
        }
        void InitializeCallback(IAsyncResult ar)
        {
            try
            {
                if (ar.IsCompleted == true)
                {
                    _lyncClient.EndInitialize(ar);

                    if (_lyncClient.State == ClientState.SignedOut)
                    {
                        string uri = CoreManager.ConfigManager.Settings.CurrentUserName;
                        string username = CoreManager.ConfigManager.Settings.CurrentUserName;
                        string password = CoreManager.ConfigManager.Settings.PassWord;
                        Common.SignUser = new User(uri, username, password);

                        _lyncClient.BeginSignIn(uri, username, password, SignInCallback, null);

                        _dispatcher.Invoke(new Action(delegate()
                        {
                            this.lblMessage.Text = "正在登录到Lync，请稍候……";
                        }), null);
                    }
                }
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - InitializeCallback", lce);
            }
            catch (Exception ex)
            {
                Logging.Add("Lync Exception - System Exception - InitializeCallback", ex);
            }
        }
        void lyncClient_SignInDelayed(object sender, SignInDelayedEventArgs e)
        {
            //_dispatcher.Invoke(new Action(delegate()
            //{
            //    try
            //    {
            //        Common.SignUser = null;
            //        _lyncClient.BeginSignOut(signOutCallback, null);

            //        lblMessage.Text = "登陆超时，请";
            //        this.btnRetry.Visibility = System.Windows.Visibility.Visible;
            //    }
            //    catch (LyncClientException ex)
            //    {
            //    }
            //}), null);
        }
        void lyncClient_CredentialRequested(object sender, CredentialRequestedEventArgs e)
        {
            try
            {
                if (e.Type == CredentialRequestedType.SignIn)
                {

                    string domainAndUsername = string.Empty;
                    string password = string.Empty;
                    _dispatcher.Invoke(new Action(delegate()
                    {
                        domainAndUsername = CoreManager.ConfigManager.Settings.CurrentUserName;
                        password = CoreManager.ConfigManager.Settings.PassWord;
                    }), null);

                    e.Submit(domainAndUsername, password, false);
                }
                else
                {
                    e.Cancel(false);
                    Common.SignUser = null;
                    _dispatcher.Invoke(new Action(delegate()
                    {
                        lblMessage.Text = "登录失败";
                        _lyncClient.BeginSignOut(signOutCallback, null);
                    }), null);
                }
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - lyncClient_CredentialRequested", lce);
            }
            catch (Exception ex)
            {
                Logging.Add("Lync Exception - System Exception - lyncClient_CredentialRequested", ex);
            }
        }
        void SignInCallback(IAsyncResult ar)
        {
            try
            {
                _lyncClient.EndSignIn(ar);
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - SignInCallback", lce);
                return;
            }

            LoadContract();
        }
        void signOutCallback(IAsyncResult ar)
        {
            try
            {
                _lyncClient.EndSignOut(ar);
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - signOutCallback", lce);
            }
        }

        void btnRetry_Click(object sender, RoutedEventArgs e)
        {
            _lyncClient.EndSignIn(null);
            if (_lyncClient.State == ClientState.SignedOut)
            {
                this.lblMessage.Text = "正在登录到Lync，请稍候……";
                this.btnRetry.Visibility = System.Windows.Visibility.Collapsed;
                _lyncClient.BeginSignIn(Common.SignUser.Uri, Common.SignUser.Username, Common.SignUser.Password, SignInCallback, null);
            }
        }

        #endregion

        #region 联系人列表

        void LoadContract()
        {
            try
            {
                if (_lyncClient.State == ClientState.SignedIn)
                {
                    _dispatcher.Invoke(new Action(delegate()
                    {
                        this.gridLoading.Visibility = System.Windows.Visibility.Hidden;
                        this.menuContractManager.IsEnabled = true;

                        this.LyncViewModel.InitClient(_lyncClient, _dispatcher);
                    }), null);

                    _lyncClient.ConversationManager.ConversationAdded += ConversationManager_ConversationAdded;
                    _lyncClient.ConversationManager.ConversationRemoved += ConversationManager_ConversationRemoved;
                }
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - LoadContract", lce);
            }
            catch (Exception ex)
            {
                Logging.Add("Lync Exception - System Exception - LoadContract", ex);
            }
        }

        #endregion

        #region 监听呼叫

        void ConversationManager_ConversationAdded(object sender, ConversationManagerEventArgs e)
        {
            try
            {
                var inviter = (Contact)e.Conversation.Properties[ConversationProperty.Inviter];
                if (inviter == null || _lyncClient.Self.Contact.Uri == inviter.Uri) return;

                if (e.Conversation.Modalities[ModalityTypes.InstantMessage].State != ModalityState.Disconnected)
                    StartIM(e.Conversation);
                else
                    e.Conversation.Modalities[ModalityTypes.InstantMessage].ModalityStateChanged += Program_ModalityStateChanged;
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - ConversationManager_ConversationAdded", lce);
            }
            catch (Exception ex)
            {
                Logging.Add("Lync Exception - System Exception - ConversationManager_ConversationAdded", ex);
            }
        }
        void StartIM(Conversation conversation)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                IList<string> contacts = new List<string>();
                IList<LyncContract> contactList = new List<LyncContract>();
                foreach (Participant participant in conversation.Participants)
                {
                    if (!participant.IsSelf)
                    {
                        contacts.Add(participant.Contact.Uri);
                        contactList.Add(new LyncContract(participant.Contact, _dispatcher));
                    }
                }

                string conversationId = (string)conversation.Properties[ConversationProperty.Id];
                ChatForm form = FormManager.GetByConversation(conversation);
                if (form == null)
                {
                    form = FormManager.GetByContacts(contacts);
                    if (form == null)
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        string callerName = ((Contact)conversation.Properties[ConversationProperty.Inviter]).GetContactInformation(ContactInformationType.DisplayName).ToString();
                        sb.Append(callerName);
                        sb.Append(Environment.NewLine);
                        sb.Append("正在邀请您加入会话，是否同意？");
                        if (System.Windows.Forms.MessageBox.Show(
                            sb.ToString()
                            , "会话邀请"
                            , System.Windows.Forms.MessageBoxButtons.YesNo
                            , System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {

                            conversation.Modalities[ModalityTypes.InstantMessage].Accept();

                            Chat chat = new Chat(contactList);
                            chat.ID = Guid.NewGuid();
                            chat._Conversation = conversation;
                            form = new ChatForm(chat.ID, contacts, chat);
                            form.ConversationId = conversationId;
                            FormManager.Add(form);
                            chat.Show();
                        }
                        else
                        {
                            conversation.End();
                        }
                    }
                    else
                    {
                        conversation.Modalities[ModalityTypes.InstantMessage].Accept();
                        form.ConversationId = conversationId;
                        form.Chat._Conversation = conversation;
                        form.Chat.AddEvent(conversation);
                        form.Chat.Focus();
                    }
                }
                else
                {
                    conversation.Modalities[ModalityTypes.InstantMessage].Accept();
                    form.ConversationId = conversationId;
                    form.Chat._Conversation = conversation;
                    form.Chat.AddEvent(conversation);
                    form.Chat.Focus();
                }
            }), null);
        }
        void Program_ModalityStateChanged(object sender, ModalityStateChangedEventArgs e)
        {
            //if (e.NewState != ModalityState.Disconnected)
            //{
            //    var modality = sender as Microsoft.Lync.Model.Conversation.AudioVideo.AVModality;

            //    StartIM(modality.Conversation);

            //    modality.Conversation.Modalities[ModalityTypes.InstantMessage].ModalityStateChanged -= Program_ModalityStateChanged;
            //}
        }
        void ConversationManager_ConversationRemoved(object sender, ConversationManagerEventArgs e)
        {

        }

        #endregion

        #region 菜单

        //添加组
        private void AddGroupBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void AddGroupBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FileNamePicker picker = new FileNamePicker(s =>
            {
                LyncViewModel.AddGroup(s);
            }, this.LyncViewModel.GroupList.Select(m => m.Name).ToArray());
            picker.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            picker.Owner = this;
            picker.ShowDialog();
        }
        //重命名组
        private void RenameGroupBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var lyncGroup = this.listGroup.SelectedItem as LyncGroup;
            if (lyncGroup != null && lyncGroup._CGroup != null && lyncGroup.Name != "Other Contacts")
            {
                e.CanExecute = true;
            }
        }
        private void RenameGroupBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var lyncGroup = this.listGroup.SelectedItem as LyncGroup;
            if (lyncGroup != null)
            {
                FileNamePicker picker = new FileNamePicker(s =>
                {
                    lyncGroup.RenameGroup(s);
                }, lyncGroup.Name, this.LyncViewModel.GroupList.Where(m => m.Name != lyncGroup.Name).Select(m => m.Name).ToArray());
                picker.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                picker.Owner = this;
                picker.ShowDialog();
            }
        }
        //删除组
        private void RemoveGroupBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var lyncGroup = this.listGroup.SelectedItem as LyncGroup;
            if (lyncGroup != null && lyncGroup._CGroup != null && lyncGroup.Name != "Other Contacts")
            {
                e.CanExecute = true;
            }
        }
        private void RemoveGroupBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var lyncGroup = this.listGroup.SelectedItem as LyncGroup;
            if (lyncGroup != null)
            {
                if (System.Windows.Forms.MessageBox.Show("确定删除该组吗？", "系统提示", 
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                {
                    LyncViewModel.RemoveGroup(lyncGroup);
                }
            }
        }

        //添加联系人
        private void AddContractBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void AddContractBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.txtSearch.Focus();
        }

        //发送即时消息
        private void SendMessageBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                e.CanExecute = true;
            }
        }
        private void SendMessageBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                this.ShowChat(item.DataContext as LyncContract);
            }
        }
        //在组织架构中发送即时消息
        private void SendMessageOnOrganizationBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if (item != null && item.DataContext != null)
            {
                OrgViewModel model = item.DataContext as OrgViewModel;
                OrgNode node = model.Current as OrgNode;
                if (node != null && !node.NodeIsDept)
                    e.CanExecute = true;
            }
        }
        private void SendMessageOnOrganizationBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_lyncClient == null || _lyncClient.State != ClientState.SignedIn)
            {
                MessageBox.Show("还未登陆到 Lync");
                return;
            }

            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if (item != null && item.DataContext != null)
            {
                OrgViewModel model = item.DataContext as OrgViewModel;
                if (!string.IsNullOrEmpty(model.Account))
                {
                    LyncContract contract = LyncViewModel.GetContractByUri(model.Account);
                    if (contract != null)
                    {
                        if (_lyncClient.Self.Contact.Uri == contract.Uri)
                        {
                            MessageBox.Show("不可以给自己发消息");
                            return;
                        }
                        this.ShowChat(contract);
                    }
                    else
                    {
                        MessageBox.Show("未在组织中找到该联系人");
                    }
                }
            }
        }
        //复制联系人
        private void CopyContractBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                e.CanExecute = true;
            }
        }
        //移动联系人
        private void MoveContractBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                e.CanExecute = true;
            }
        }
        //从组中删除联系人
        private void RemoveContractFromGroupBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                e.CanExecute = true;
            }
        }
        private void RemoveContractFromGroupBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                if (System.Windows.Forms.MessageBox.Show("确定删除该联系人吗？", "系统提示",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                {
                    LyncContract contract = item.DataContext as LyncContract;
                    if (contract._Group != null)
                        contract._Group.RemoveContract((item.DataContext as LyncContract)._Contact);
                }
            }
        }
        //删除联系人
        private void RemoveContractBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                e.CanExecute = true;
            }
        }
        private void RemoveContractBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                if (System.Windows.Forms.MessageBox.Show("确定删除该联系人吗？", "系统提示",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                {
                    LyncViewModel.RemoveContract((item.DataContext as LyncContract)._Contact);
                }
            }
        }
        //添加到联系人列表
        private void AddContractToListBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                e.CanExecute = true;
            }
        }
        //添加到联系人分组
        private void AddContractToGroupBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                e.CanExecute = this.ExistsContract(e.Parameter.ToString(), (item.Content as LyncContract).Uri);
            }
        }
        private void AddContractToGroupBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                var group = LyncViewModel.GroupList.Where(m => m.Name == e.Parameter.ToString()).FirstOrDefault();
                if (group != null)
                {
                    group.AddContract((item.DataContext as LyncContract)._Contact);
                    this.txtSearch.Text = "按名称、电子邮件地址搜索联系人";
                }
            }
        }

        //复制到联系人分组
        private void CopyContractToGroupBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                e.CanExecute = this.ExistsContract(e.Parameter.ToString(), (item.Content as LyncContract).Uri);
            }
        }
        private void CopyContractToGroupBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                var group = LyncViewModel.GroupList.Where(m => m.Name == e.Parameter.ToString()).FirstOrDefault();
                if (group != null)
                {
                    group.AddContract((item.DataContext as LyncContract)._Contact);
                }
            }
        }

        //移动到联系人分组
        private void MoveContractToGroupBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                e.CanExecute = this.ExistsContract(e.Parameter.ToString(), (item.Content as LyncContract).Uri);
            }
        }
        private void MoveContractToGroupBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ListBoxItem item = e.OriginalSource as ListBoxItem;
            if (item != null && item.DataContext != null)
            {
                var newGroup = LyncViewModel.GroupList.Where(m => m.Name == e.Parameter.ToString()).FirstOrDefault();
                if (newGroup != null)
                {
                    LyncContract contract = item.DataContext as LyncContract;
                    newGroup.AddContract(contract._Contact);
                    var oldGroup = LyncViewModel.GroupList.Where(m => m.Name == contract._Group.Name).FirstOrDefault();
                    if (oldGroup != null)
                    {
                        oldGroup.RemoveContract(contract._Contact);
                    }
                }
            }
        }

        //检测组中是否存在某联系人
        bool ExistsContract(string groupName, string uri)
        {
            var group = LyncViewModel.GroupList.Where(m => m.Name == groupName).FirstOrDefault();
            if (group != null)
            {
                var contract = group.ContractList.Where(m => m.Uri == uri).FirstOrDefault();
                if (contract == null)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 搜索联系人

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.pnlGroupList != null && this.pnlSearchList != null)
            {
                if (!string.IsNullOrEmpty(this.txtSearch.Text) &&
                    this.txtSearch.Text != "按名称、电子邮件地址搜索联系人")
                {
                    this.pnlGroupList.Visibility = Visibility.Collapsed;
                    this.pnlSearchList.Visibility = Visibility.Visible;
                    LyncViewModel.SearchContracts(this.txtSearch.Text);
                }
                else
                {
                    this.pnlGroupList.Visibility = Visibility.Visible;
                    this.pnlSearchList.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.txtSearch.Text == "按名称、电子邮件地址搜索联系人")
            {
                this.txtSearch.Text = "";
            }
        }
        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtSearch.Text))
            {
                this.txtSearch.Text = "按名称、电子邮件地址搜索联系人";
            }
        }

        #endregion

        //聊天窗体
        void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            if (item.DataContext != null)
            {
                LyncContract contract = item.DataContext as LyncContract;
                this.ShowChat(contract);
            }
        }

        void ShowChat(LyncContract contract)
        {
            IList<string> contacts = new List<string>() { contract.Uri };
            ChatForm form = FormManager.GetByContacts(contacts);
            if (form != null)
            {
                form.Chat.Focus();
            }
            else
            {
                Chat chat = new Chat(new List<LyncContract>() { contract });
                chat.ID = Guid.NewGuid();
                form = new ChatForm(chat.ID, contacts, chat);
                FormManager.Add(form);
                chat.Show();
            }
        }

        void lyncClient_StateChanged(object sender, ClientStateChangedEventArgs e)
        {
            try
            {
                if (_lyncClient.State == ClientState.Uninitialized)
                {
                    _lyncClient.BeginInitialize(InitializeCallback, null);
                }
                else if (_lyncClient.State == ClientState.SignedOut)
                {
                    if (Common.SignUser == null)
                    {
                        Common.SignUser = new User(
                                   CoreManager.ConfigManager.Settings.CurrentUserName,
                                   CoreManager.ConfigManager.Settings.CurrentUserName,
                                   CoreManager.ConfigManager.Settings.PassWord);
                    }
                    _lyncClient.BeginSignIn(Common.SignUser.Uri, Common.SignUser.Username, Common.SignUser.Password, SignInCallback, null);
                }
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - lyncClient_StateChanged", lce);
            }
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                Common.CloseLyncProcess();
                Application.Current.Shutdown();
            }
        }

        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
            listBox.RaiseEvent(eventArg);
        }

        #endregion

        private void GoUp_Click(object sender, RoutedEventArgs e)
        {
            if (_backwardNavigations.Count > 0)
            {
                var upFolderPath = _backwardNavigations.Pop();
                CurrentDirectory = upFolderPath;
            }
        }

        // Listview item上下文菜单命令 判断是否能执行删除操作
        private void DelBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool canExecute = this.FileListView.SelectedItem != null;
            e.CanExecute = canExecute;
        }

        //执行删除操作命令
        private void DelBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItem = this.FileListView.SelectedItem as SkyDriveItemModel;
            if (selectedItem != null)
            {
                if (selectedItem.IsDirectory)
                    this._sm.PersonalServer.DeleteFolder(selectedItem.ServerItem, selectedItem.FullPath);
                else
                    _sm.PersonalServer.DeleteFile(selectedItem.ServerItem, selectedItem.FullPath);

                CurrentDirectory = CurrentDirectory;
            }
        }

        private string ResolveRelativePath(string path)
        {
            string relPath = string.Empty;
            var relUrl = CurrentDirectory.FullPath.Replace(_rootDir.FullPath, string.Empty).TrimStart('/');
            if (!string.IsNullOrEmpty(relUrl))
                relPath = relUrl.TrimEnd('/') + "/" + path;
            else
                relPath = path;

            return relPath;
        }

        private void NewFolder()
        {
            FileNamePicker picker = new FileNamePicker(s =>
            {
                string createdPath = ResolveRelativePath(s);

                SkyDriveItemModel currentDirModel = CurrentDirectory as SkyDriveItemModel;
                ServerItem parentServerItem = currentDirModel.ServerItem;

                _sm.PersonalServer.CreateNewFolder(parentServerItem, createdPath.Replace("/", "\\"));
                CurrentDirectory = CurrentDirectory;
            }, ListViewItems.Select(s => s.Name).ToArray());
            picker.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            picker.Owner = this;
            picker.ShowDialog();
        }

        private void UploadBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog().Value)
            {
                var fi = new System.IO.FileInfo(ofd.FileName);
                ClientItem ci = new ClientItem(fi);
                string selectedFileName = System.IO.Path.GetFileName(ofd.FileName);
                string fileOnServerPath = ResolveRelativePath(selectedFileName);

                UploadingItem files = new UploadingItem();
                //files.Add(new UploadFileModel() { Name = ci.BasicInfo.Name, FullPath = ci.BasicInfo.FullName });
                files.Add(new UploadFileModel(ci, fi.Directory.FullName));

                FancyBalloon balloon = new FancyBalloon();
                balloon.BalloonText = "上传文件";
                balloon.UploadingItems = files;
                //balloon.UploadingListboxItems = files;
                //show and close after 2.5 seconds
                tb.ShowCustomBalloon(balloon, PopupAnimation.Slide, null);

                SkyDriveItemModel currentDirModel = CurrentDirectory as SkyDriveItemModel;
                ServerItem parentServerItem = currentDirModel.ServerItem;

                Task uploadtask = new Task(() =>
                {
                    foreach (UploadFileModel file in files)
                    {
                        file.Status = "正在上传...";
                        _sm.PersonalServer.CreateNewFile(ci, parentServerItem, fileOnServerPath, b =>
                        {
                            // b 上传完成 是否有错误
                            if (!b)
                            {
                                this.Dispatcher.Invoke(new Action(() =>
                                {
                                    Thread.Sleep(100);
                                    file.Status = "上传完成";
                                    CurrentDirectory = CurrentDirectory;
                                }));
                            }
                        });
                    }
                });
                uploadtask.Start();
            }
        }

        private void FileListCtmRefresh_Click(object sender, RoutedEventArgs e)
        {
            CurrentDirectory = CurrentDirectory;
        }

        // 临时测试代码
        private void OrgManage_Click(object sender, RoutedEventArgs e)
        {
            OrgManage om = new OrgManage();
            om.Show();
        }



        #region 列表导航路由事件
        private void NavHomeButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentDirectory = _rootDir;
            _backwardNavigations.Clear();
            _forwardNavigations.Clear();
        }

        private void NavBackCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_backwardNavigations.Count > 0)
                e.CanExecute = true;
        }

        private void NavBackCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_backwardNavigations.Count > 0)
            {
                var upFolderPath = _backwardNavigations.Pop();
                _forwardNavigations.Push(CurrentDirectory);
                CurrentDirectory = upFolderPath;
            }
        }

        private void NavForwardCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_forwardNavigations.Count > 0)
                e.CanExecute = true;
        }

        private void NavForwardCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_forwardNavigations.Count > 0)
            {
                var forwardFolderPath = _forwardNavigations.Pop();
                _backwardNavigations.Push(CurrentDirectory);
                CurrentDirectory = forwardFolderPath;
            }
        }

        private void SharingCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        

        private void SharingCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //SkyDriveItemModel orgSource = (SkyDriveItemModel)e.OriginalSource;
            //var serverItem = orgSource.ServerItem;

            SkyDriveItemModel item = this.FileListView.SelectedItem as SkyDriveItemModel;

            var sharingWindow = new ShareWindow(_sm.PersonalServer, item.ServerItem,
                () =>
                {
                    this.CurrentDirectory = this.CurrentDirectory;
                }/*完成分享后的回调函数，刷新当前列表*/);
            sharingWindow.Owner = this;
            sharingWindow.ShowDialog();
        }

       
        #endregion

        #region 文件上传
        private void UploadFile(ServerSide server,IEntryModel entry,Action action=null)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog().Value)
            {
                var fi = new System.IO.FileInfo(ofd.FileName);
                ClientItem ci = new ClientItem(fi);
                string selectedFileName = System.IO.Path.GetFileName(ofd.FileName);
                string fileOnServerPath = ResolveRelativePath(selectedFileName);

                UploadingItem files = new UploadingItem();
                //files.Add(new UploadFileModel() { Name = ci.BasicInfo.Name, FullPath = ci.BasicInfo.FullName });
                files.Add(new UploadFileModel(ci, fi.Directory.FullName));

                FancyBalloon balloon = new FancyBalloon();
                balloon.BalloonText = "上传文件";
                balloon.UploadingItems = files;
                //balloon.UploadingListboxItems = files;
                //show and close after 2.5 seconds
                tb.ShowCustomBalloon(balloon, PopupAnimation.Slide, null);

                SkyDriveItemModel currentDirModel = entry as SkyDriveItemModel;
                ServerItem parentServerItem = currentDirModel.ServerItem;

                Task uploadtask = new Task(() =>
                {
                    foreach (UploadFileModel file in files)
                    {
                        file.Status = "正在上传...";
                        server.CreateNewFile(ci, parentServerItem, fileOnServerPath, b =>
                        {
                            // b 上传完成 是否有错误
                            if (!b)
                            {
                                this.Dispatcher.Invoke(new Action(() =>
                                {
                                    Thread.Sleep(100);
                                    file.Status = "上传完成";

                                    if (action != null)
                                        action();
                                }));
                            }
                            else
                            {
                                this.Dispatcher.Invoke(new Action(() =>
                                {
                                    file.Status = "上传错误";
                                }));
                            }
                        });
                    }
                });
                uploadtask.Start();
            }
        }

        private void ListViewUploadFile_Click(object sender, RoutedEventArgs e)
        {
            UploadFile(_sm.PersonalServer,CurrentDirectory,()=>
            {
                CurrentDirectory = CurrentDirectory;
            });
        }

        private void ListViewUploadFile2_Click(object sender, RoutedEventArgs e)
        {
            UploadFile(_sm.CompanyServerSide, CompanyCurrentDirectory, () =>
                {
                    CompanyCurrentDirectory = CompanyCurrentDirectory;
                });
        }

        
        #endregion

        #region 文件复制\粘贴
        private void CopyBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var selectedItem = this.FileListView.SelectedItem;
            if (selectedItem != null)
            {
                SkyDriveItemModel model = selectedItem as SkyDriveItemModel;
                e.CanExecute = !model.IsDirectory;
            }
        }

        private void CopyBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var selectedItem = this.FileListView.SelectedItem;
                if (selectedItem != null)
                {
                    SkyDriveItemModel model = selectedItem as SkyDriveItemModel;
                    Clipboard.SetText(ClipboardTextPrefix + model.ServerItem.Id + "|" + model.ServerItem.Name);
                }
            }
            catch { }
        }

        private void PasteBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                var clipText = Clipboard.GetText();
                if (clipText.StartsWith(ClipboardTextPrefix))
                {
                    e.CanExecute = true;
                }
            }
        }

        private void PasteBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var param = e.Parameter;

            var clipText = Clipboard.GetText();
            var paramText = clipText.Substring(ClipboardTextPrefix.Length);
            var id = Convert.ToInt32(paramText.Split(new char[] { '|' })[0]);
            var name = paramText.Split(new char[] { '|' })[1];

            string relPath = string.Empty;
            var relUrl = CurrentDirectory.FullPath.Replace(_rootDir.FullPath, string.Empty).TrimStart('/');
            if (!string.IsNullOrEmpty(relUrl))
                relPath = relUrl.TrimEnd('/') + "/" + name;
            else
                relPath = name;

            string serverRelPath = _sm.PersonalServer.RootFolderServerRelativeUrl + "/" + relPath.Replace('\\', '/');

            SP.ListItem copiedItem = _sm.PersonalServer.RemoteLibrary.GetItemById(id);
            SP.File copiedFile = copiedItem.File;
            copiedFile.CopyTo(serverRelPath, true);

            System.Threading.Thread.Sleep(300);
            CurrentDirectory = CurrentDirectory;
        }
        #endregion

        #region  公司空间导航事件
        private void CompanyNavBackCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_backwardCompanyNavs.Count > 0)
                e.CanExecute = true;
        }

        private void CompanyNavBackCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_backwardCompanyNavs.Count > 0)
            {
                var upFolderPath = _backwardCompanyNavs.Pop();
                _forwardCompanyNavs.Push(CompanyCurrentDirectory);
                CompanyCurrentDirectory = upFolderPath;
            }
        }

        private void CompanyNavForwardCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_forwardCompanyNavs.Count > 0)
                e.CanExecute = true;
        }

        private void CompanyNavForwardCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_forwardCompanyNavs.Count > 0)
            {
                var forwardFolderPath = _forwardCompanyNavs.Pop();
                _backwardCompanyNavs.Push(CompanyCurrentDirectory);
                CompanyCurrentDirectory = forwardFolderPath;
            }
        }
        private void CompanyHomeButton_Click(object sender, RoutedEventArgs e)
        {
            CompanyCurrentDirectory = _companyRootDir;
            _backwardCompanyNavs.Clear();
            _forwardCompanyNavs.Clear();
        } 
        #endregion

        Stack<IEntryModel> _backwardCompanyNavs = new Stack<IEntryModel>();
        Stack<IEntryModel> _forwardCompanyNavs = new Stack<IEntryModel>();

        private void CompanyFileListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            var selectedItem = item.Content as SkyDriveItemModel;
            if (selectedItem != null)
            {
                if (selectedItem.IsDirectory)
                {
                    _backwardCompanyNavs.Push(CompanyCurrentDirectory);
                    CompanyCurrentDirectory = selectedItem;
                }
            }
        }
    }
}
