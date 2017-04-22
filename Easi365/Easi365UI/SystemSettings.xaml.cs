using Easi365UI.Windows.Controls;
using System;
using System.Windows;
using ClientLib.Core;
using System.Windows.Input;

namespace Easi365UI
{
    /// <summary>
    /// SystemSettings.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSettings : EasiWindow
    {
        public SystemSettings()
        {
            InitializeComponent();

            //点击窗体头部区域拖拽整个窗体移动
            this.ContentHeader.MouseLeftButtonDown += HeaderGrid_MouseLeftButtonDown;
        }

        private void SystemSettingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitSettings();
        }

        private void InitSettings()
        {
            //服务器类型
            CoreManager.ServerMode mode = (CoreManager.ServerMode)Enum.Parse(typeof(CoreManager.ServerMode), CoreManager.ConfigManager.Settings.ServerMode, true);
            switch (mode)
            {
                case CoreManager.ServerMode.Office365:
                    this.rdoOffice365.IsChecked = true; break;
                case CoreManager.ServerMode.Local:
                    this.rdoLocal.IsChecked = true; break;
                default:
                    this.rdoOffice365.IsChecked = true; break;
            }

            //服务器地址
            this.txtServerUrl.Text = CoreManager.ConfigManager.Settings.ServerUrl;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //服务器类型
            if (this.rdoOffice365.IsChecked == true)
            {
                if (!string.IsNullOrEmpty(this.txtServerUrl.Text))
                    CoreManager.ConfigManager.Settings.Office365ServerUrl = this.txtServerUrl.Text;
                CoreManager.ConfigManager.Settings.ServerMode = CoreManager.ServerMode.Office365.ToString();
            }
            else if (this.rdoLocal.IsChecked == true)
            {
                if (!string.IsNullOrEmpty(this.txtServerUrl.Text))
                    CoreManager.ConfigManager.Settings.LocalServerUrl = this.txtServerUrl.Text;
                CoreManager.ConfigManager.Settings.ServerMode = CoreManager.ServerMode.Local.ToString();
            }

            //服务器地址
            if (!string.IsNullOrEmpty(this.txtServerUrl.Text))
                CoreManager.ConfigManager.Settings.ServerUrl = this.txtServerUrl.Text;

            //持久化数据
            CoreManager.ConfigManager.SaveSettings();

            this.Close();
        }

        private void rdoOffice365_Click(object sender, RoutedEventArgs e)
        {
            if (this.rdoOffice365.IsChecked == true)
                this.txtServerUrl.Text = CoreManager.ConfigManager.Settings.Office365ServerUrl;
        }

        private void rdoLocal_Click(object sender, RoutedEventArgs e)
        {
            if (this.rdoLocal.IsChecked == true)
                this.txtServerUrl.Text = CoreManager.ConfigManager.Settings.LocalServerUrl;
        }

        //最小化登录窗体
        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        //关闭登录窗体
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void HeaderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
