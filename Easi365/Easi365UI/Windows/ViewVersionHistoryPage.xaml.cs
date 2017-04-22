using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using mshtml;

namespace Easi365UI.Windows
{
    /// <summary>
    /// ViewVersionHistoryPage.xaml 的交互逻辑
    /// </summary>
    public partial class ViewVersionHistoryPage : Window
    {
        string _remoteUrl;
        string _title;

        public ViewVersionHistoryPage()
        {
            InitializeComponent();

            this.versionHistoryViewr.LoadCompleted += versionHistoryViewr_LoadCompleted;
        }

        void versionHistoryViewr_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

            //Versions.aspx
            //如果当前页面是版本历史记录页面 则执行脚本 修正sharepoint页面IsDlg s4-workspace没有高度的情况
            if (e.Uri.LocalPath.IndexOf("Versions.aspx") > -1)
            {
                var doc = this.versionHistoryViewr.Document as HTMLDocument;
                if (doc != null)
                {
                    IHTMLWindow2 window = doc.parentWindow;
                    string js = @"function setWorkSpaceHeight(){
                                var workspace = document.getElementById('s4-workspace');
                                workspace.style.height = '350px';
                            }
                                setWorkSpaceHeight();
                            ";

                    window.execScript(js, "javascript");
                }
            }
        }

        public ViewVersionHistoryPage(string url,string title)
            : this()
        {
            this._remoteUrl = url;
            this._title = title;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Title = _title;
                this.versionHistoryViewr.Navigate(_remoteUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载页面失败.");
            }
        }
    }
}
