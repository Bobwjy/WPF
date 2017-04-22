using ClientLib.Core;
using Easi365UI.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace Easi365UI
{
    static class Program
    {
        //static void Activate(string title)
        //{
        //    //Find the window, using the Window Title
        //    IntPtr hWnd = Win32.FindWindow(null, title);
        //    if (hWnd.ToInt32() > 0) //If found
        //    {
        //        Win32.SetForegroundWindow(hWnd); //Activate it
        //    }
        //}

        static List<string> MissingDlls = new List<string>() 
            { 
                "System.IO.dll",
                "System.Runtime.dll", 
                "System.Threading.Tasks.dll" 
            };

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
        private const int SW_MAXIMIZE = 3;
        private const int SW_SHOWNORMAL = 1;

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var asmDir = currentDirectory + "Asm3trd\\";

            MissingDlls.ForEach(c =>
            {
                try
                {
                    FileInfo dllInfo = new FileInfo(currentDirectory + c);

                    if (!dllInfo.Exists)
                        File.Copy(asmDir + c, currentDirectory + c);
                }
                catch (Exception ex)
                {
                    Logging.Add("复制应用程序DLL文件失败.", ex);
                    MessageBox.Show("程序初始化失败.");
                }
            });

            // app name Easi365UI
            var thisProc = Process.GetCurrentProcess();
            bool isAppNotStarted = false;
            Mutex mLocker = new Mutex(true, "Easi365", out isAppNotStarted);

            if (isAppNotStarted)
            {
                mLocker.WaitOne();

                //初始化核心
                CoreManager.Initialize(
                     Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                     "Easi365" + Path.DirectorySeparatorChar));

                App.databasePath = CoreManager.ConfigManager.Settings.DbPath + Path.DirectorySeparatorChar + "EasiDB.mdb";

                Easi365UI.App app = new Easi365UI.App();
                app.InitializeComponent();
                app.Run();
                mLocker.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("程序正在运行.");
               // Application.Current.Shutdown();
                Environment.Exit(0);
                return;
                //var procs = Process.GetProcessesByName("Easi365UI");
                //foreach (var p in procs.Where(p => p.MainWindowHandle != IntPtr.Zero))
                //{
                //    ShowWindow(p.MainWindowHandle, SW_MAXIMIZE);
                //    Application.Current.Shutdown();
                //    return;
                //}
            }
        }
    }
}
