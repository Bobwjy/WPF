using ClientLib.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using Easi365UI.Lync;
using Easi365UI.Windows;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ClientLib.Core;
using System.IO;

namespace Easi365UI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static ICredentials spCredentials = null;
        //得到屏幕整体宽度
        public static double screenX = SystemParameters.PrimaryScreenWidth;
        //得到屏幕整体高度
        public static double screenY = SystemParameters.PrimaryScreenHeight;
        //站点根目录 --> CoreManager.ConfigManager.Settings.ServerUrl
        //public const string  _serverRoot = "https://easi365.sharepoint.com/";
        //邮件操作类
        public static EmailService _es;
        //未读邮件数
        public static string _unreadMailCount;

        public static string databasePath { get; set; }

        public UploadingFileDetailPage UploadingFileDetailPage { get; private set; }

        public static App CurrentApp
        {
            get
            {
                return (App)App.Current;
            }
        }


        /// <summary>
        /// 主窗体
        /// </summary>
        public static MaxWindow Easi365MainWindow;

        public App()
        {
            UploadingFileDetailPage = new UploadingFileDetailPage();
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
        private const int SW_MAXIMIZE = 3;
        private const int SW_SHOWNORMAL = 1;

        private static Mutex singleInstanceMutex = new Mutex(true, "Easi365UI");

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //if (singleInstanceMutex.WaitOne(TimeSpan.Zero, true)) {
            //    CoreManager.Initialize(
            //             Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            //             "Easi365" + Path.DirectorySeparatorChar));

            //    App.databasePath = CoreManager.ConfigManager.Settings.DbPath + Path.DirectorySeparatorChar + "EasiDB.mdb";
            //}
            //else
            //{
            //    var procs = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            //    foreach (var p in procs.Where(p => p.MainWindowHandle != IntPtr.Zero))
            //    {
            //        ShowWindow(p.MainWindowHandle, SW_MAXIMIZE);
            //        Application.Current.Shutdown();
            //        return;
            //    }
            //}
        }
    }
}
