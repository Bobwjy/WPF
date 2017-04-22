using ClientLib;
using ClientLib.Core;
using ClientLib.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Easi365UI.Windows
{
    /// <summary>
    /// ViewDocPage.xaml 的交互逻辑
    /// </summary>
    public partial class ViewDocPage : Window
    {
        FrameworkElement _placementTarget;
        ServerSide _server = null;
        string _filePath;
        ServerItem _si;
        string _status;
       

        public WebBrowser WebBrowser { get { return officeDocViewr; } }

        public ViewDocPage()
        {
            InitializeComponent();
        }

        public ViewDocPage(ServerSide server, ServerItem si,string Status)
            : this()
        {
            _status = Status;
            _server = server;
            _si = si;
            _filePath = si.ServerRelativeUrl;
        }

        public ViewDocPage(FrameworkElement placementTarget)
            : this()
        {
            _placementTarget = placementTarget;
            Window owner = Window.GetWindow(placementTarget);
            //Debug.Assert(owner != null);

            //owner.SizeChanged += delegate { OnSizeLocationChanged(); };
            owner.LocationChanged += delegate { OnSizeLocationChanged(); };
            _placementTarget.SizeChanged += delegate { OnSizeLocationChanged(); };

            if (owner.IsVisible)
            {
                Owner = owner;
                Show();
            }
            else
                owner.IsVisibleChanged += delegate
                {
                    if (owner.IsVisible)
                    {
                        Owner = owner;
                        Show();
                    }
                };

            //owner.LayoutUpdated += new EventHandler(OnOwnerLayoutUpdated);
        }

        

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_server != null)
            {
                this.ResizeMode = System.Windows.ResizeMode.CanResize;
                this.WindowStyle = System.Windows.WindowStyle.ToolWindow;

                CoreManager.ServerMode mode = (CoreManager.ServerMode)Enum.Parse(typeof(CoreManager.ServerMode), CoreManager.ConfigManager.Settings.ServerMode, true);
                switch (mode)
                {
                    case CoreManager.ServerMode.Office365:
                        {
                            string url = string.Format(ViewDocument.Office365ViewDocPageUrlPattern,
                                _server.ClientCtx.Url,
                               "{" + _si.UniqueId + "}",
                               _si.Name,
                               "edit");
                            this.officeDocViewr.Navigate(url);
                        }
                        break;
                    case CoreManager.ServerMode.Local:
                        {
                            try
                            {
                                if (_status == "Edit")
                                {
                                    string html = await _server.DocumentViewr.GetEditDocContentAsync(_filePath);
                                    this.officeDocViewr.NavigateToString(html);
                                }
                                else
                                {
                                    string html = await _server.DocumentViewr.GetViewDocContentAsync(_filePath);
                                    this.officeDocViewr.NavigateToString(html);
                                }     
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("打开文档错误.");
                                Logging.Add("在线编辑文档出错.", ex);
                            }
                        }
                        break;
                    default:
                        return;
                }


            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
           // if (!e.Cancel)
                // Delayed call to avoid crash due to Window bug.
                //Dispatcher.BeginInvoke((Action)delegate
                //{
                //    if (Owner != null)
                //        Owner.Close();
                //});
        }

        public void OnSizeLocationChanged()
        {
            Point offset = _placementTarget.TranslatePoint(new Point(), Owner);
            Point size = new Point(_placementTarget.ActualWidth, _placementTarget.ActualHeight);
            HwndSource hwndSource = (HwndSource)HwndSource.FromVisual(Owner);
            CompositionTarget ct = hwndSource.CompositionTarget;
            offset = ct.TransformToDevice.Transform(offset);
            size = ct.TransformToDevice.Transform(size);

            Win32.POINT screenLocation = new Win32.POINT(offset);
            Win32.ClientToScreen(hwndSource.Handle, ref screenLocation);
            Win32.POINT screenSize = new Win32.POINT(size);

            Win32.MoveWindow(((HwndSource)HwndSource.FromVisual(this)).Handle, screenLocation.X, screenLocation.Y, screenSize.X, screenSize.Y, true);
        }

    }
}
