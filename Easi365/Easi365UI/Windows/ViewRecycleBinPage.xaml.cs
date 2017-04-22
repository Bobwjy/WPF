﻿using System;
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
    /// ViewRecycleBinPage.xaml 的交互逻辑
    /// </summary>
    public partial class ViewRecycleBinPage : Window
    {
        string _recycleBinUrl;

        public ViewRecycleBinPage()
        {
            InitializeComponent();

            this.recycleBinViewr.LoadCompleted += recycleBinViewr_LoadCompleted;
        }

        public ViewRecycleBinPage(string url)
            : this()
        {
            _recycleBinUrl = url;
        }

        void recycleBinViewr_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var doc = this.recycleBinViewr.Document as HTMLDocument;
            if (doc != null)
            {
                IHTMLWindow2 window = doc.parentWindow;
                string js = @"function setWorkSpaceHeight(){
                                var workspace = document.getElementById('s4-workspace');
                                workspace.style.height = '320px';
                            }

                            function reSetNextPageFormAction(){
                                var usrPage = document.forms['usrpage'];
                                usrPage.action = 'RecycleBin.aspx?IsDlg=1';
                            }
                                setWorkSpaceHeight();
                            reSetNextPageFormAction();
                            ";

                window.execScript(js, "javascript");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.recycleBinViewr.Navigate(_recycleBinUrl);
        }
    }
}
