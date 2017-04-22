using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ClientLib.Utilities;

namespace ClientLib.Core
{
    public class CoreManager
    {
        private CoreManager() { }

        /// <summary>
        /// 起始路径
        /// </summary>
        public static string StartupPath { get; set; }

        /// <summary>
        /// 程序是否启动
        /// </summary>
        //public static bool IsAppStarted { get; set; }

        /// <summary>
        /// 配置管理器
        /// </summary>
        public static ConfigManager ConfigManager { get; private set; }

        /// <summary>
        /// 上传任务管理器
        /// </summary>
        public static TaskManager TaskManager { get; private set; }

        public static string LoginNamePrefix
        {
            get 
            {
                string loginNamePrefix;
                CoreManager.ServerMode mode = (CoreManager.ServerMode)Enum.Parse(typeof(CoreManager.ServerMode), CoreManager.ConfigManager.Settings.ServerMode, true);
                switch (mode)
                {
                    case CoreManager.ServerMode.Office365:
                        loginNamePrefix = Constants.Sharing.Office365LoginNamePrefix;
                        break;
                    case CoreManager.ServerMode.Local:
                        loginNamePrefix = Constants.Sharing.LoginNamePrefix;
                        break;
                    default:
                        loginNamePrefix = "";
                        break;
                }
                return loginNamePrefix;
            }
        }

        /// <summary>
        /// 登录模式（Office365和本地）
        /// </summary>
        public enum ServerMode
        {
            Office365,
            Local
        }

        /// <summary>
        /// 视图模式
        /// </summary>
        public enum ViewMode
        {
            Details,
            Thumb
        }

        /// <summary>
        /// 侧边栏导航
        /// </summary>
        public enum CollapseMode
        {
            Collapse,
            Expansion
        }

        /// <summary>
        /// 空间类型
        /// </summary>
        public enum SpaceCategory
        {
            Company,
            CooSpace,
            DeptSpace,
            SubWebSpace
        }

        public enum CooSpaceAction { 
            Applicant,
            New,
            Edit,
            View
        }

        /// <summary>
        /// 文件监控
        /// </summary>
        public static FileWatcher FileWatcher { get; private set; }

        /// <summary>
        /// 本地日志同步管理
        /// </summary>
        public static SyncLogManager SyncLogManager { get;private set; }

        public static void Initialize(string startupFolderPath)
        {
            StartupPath = startupFolderPath;

            //如果不存在启动文件夹则创建
            if (!Directory.Exists(startupFolderPath))
                Directory.CreateDirectory(startupFolderPath);

            //全局设置
            ConfigManager = new ConfigManager();
            ConfigManager.LoadSettings();
            //日志工具
            Logging.Initialize();

            //初始化数据库
            if (!File.Exists(ConfigManager.Settings.DbPath + Path.DirectorySeparatorChar + "EasiDB.mdb"))
                InitSyncDB();

            //文件目录
            if (!Directory.Exists(ConfigManager.Settings.FilesFolder))
                Directory.CreateDirectory(ConfigManager.Settings.FilesFolder);
            if (!Directory.Exists(ConfigManager.Settings.PersonalFilesFolder))
                Directory.CreateDirectory(ConfigManager.Settings.PersonalFilesFolder);
            if (!Directory.Exists(ConfigManager.Settings.PersonalFilesCache))
                Directory.CreateDirectory(ConfigManager.Settings.PersonalFilesCache);
            if (!Directory.Exists(ConfigManager.Settings.CompanyFilesCache))
                Directory.CreateDirectory(ConfigManager.Settings.CompanyFilesCache);

            //加载所有上传任务
            TaskManager = new TaskManager();
            TaskManager.LoadAllTask();

            //文件监控 本地文件保存后自动更新对应服务器端文件
            FileWatcher = new FileWatcher(ConfigManager.Settings.PersonalFilesFolder);
            FileWatcher.StartMonitoring();

            //本地日志同步管理初始化
            //SyncLogManager = new SyncLogManager();
            //SyncLogManager.StartSyncLogTask();
        }

        private static void InitSyncDB()
        {
            string emptydb = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "DataBase\\EasiDB.mdb");
            string targetpath = ConfigManager.Settings.DbPath;

            if (!Directory.Exists(targetpath))
                Directory.CreateDirectory(targetpath);

            File.Copy(emptydb, targetpath + Path.DirectorySeparatorChar + "EasiDB.mdb");
        }
    }
}
