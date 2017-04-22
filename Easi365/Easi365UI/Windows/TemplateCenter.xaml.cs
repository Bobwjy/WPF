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
using ClientLib.Core;
using ClientLib;

namespace Easi365UI.Windows
{
    /// <summary>
    /// TemplateCenter.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateCenter : Window
    {
        private string _folder;
        private ServerSide _server;
        private readonly string TemplateCenterUriPattern = "{0}/_layouts/15/TemplateCenter/Create.aspx?Folder={1}&IsDlg=1";

        public Action ClosingAction { get; set; }

        public TemplateCenter()
        {
            InitializeComponent();
        }

        public TemplateCenter(ServerSide server,string folder)
            : this()
        {
            this._folder = folder;
            this._server = server;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (ClosingAction != null)
                ClosingAction();

            base.OnClosing(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = string.Format(TemplateCenterUriPattern,_server.ClientCtx.Url,_folder);
                this.TemplateCenterWebBrowser.Navigate(new Uri(url));
            }
            catch (Exception ex)
            {
                MessageBox.Show("文档中心加载失败.");
                Logging.Add("模版中心加载失败.", ex);
            }
        }
    }
}
