using ClientLib.Entities;
using ClientLib.SyncActions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using Microsoft.SharePoint.Client;
using ClientLib.Core;
using ClientLib.Utilities;

namespace ClientLib
{
    public class SyncManager
    {
        //internal ClientSide Client { get; private set; }
        //public ServerSide Server { get; private set; }
        public LocalDB DB { get; private set; }

        /// <summary>
        /// 公司空间
        /// </summary>
        //public ServerSide CompanyServerSide { get;private set; }
        /// <summary>
        /// 个人空间
        /// </summary>
        public ServerSide PersonalServer { get; private set; }
        public ServerSide DepartmentServer { get; private set; }
        public ServerSide CompanyServer { get; private set; }
        /// <summary>
        /// 协作空间
        /// </summary>
        //public List<ServerSide> CollaborationServers { get; private set; }
        //public CollaborationServerManager CollaborationServerManager { get; private set; }
        /// <summary>
        /// 子网站空间管理器
        /// </summary>
        //public SubWebServerManager SubWebServerManager { get; private set; }
        /// <summary>
        /// 子网站空间
        /// </summary>
        //public ServerSide SubWebServerSide { get; private set; }

        public SyncManager(string serverUrl, string databasePath, string username, string password)
        {
            DB = new LocalDB(databasePath);
            //Server = new ServerSide(serverUrl, DB, username, password);
        }

        public SyncManager(string serverUrl, string databasePath, ICredentials credentials)
        {
            DB = new LocalDB(databasePath);
            //Server = new ServerSide(serverUrl, DB, credentials);
            //string personalRoot;


            PersonalServer = new PersonalServerSide(serverUrl + "ps", DB, credentials, serverUrl);
            DepartmentServer = new PersonalServerSide(serverUrl + "ds", DB, credentials, serverUrl);
            CompanyServer = new PersonalServerSide(serverUrl + "cs", DB, credentials, serverUrl);
        }

        public void Init()
        {
            //1. 初始化数据字典
            DB.LocalDict = DB.GetFilesDict();

            //2. 初始化服务器this.Server.Init();
            this.PersonalServer.Init();
            this.DepartmentServer.Init();
            this.CompanyServer.Init();

        }

        private string FormatPersonalUrl(string serverUrl, ICredentials credentials, out string personalRoot)
        {
            UrlHelper urlHelper = null;
            CoreManager.ServerMode mode = (CoreManager.ServerMode)Enum.Parse(typeof(CoreManager.ServerMode), CoreManager.ConfigManager.Settings.ServerMode, true);
            switch (mode)
            {
                case CoreManager.ServerMode.Office365:
                    urlHelper = new Office365UrlHelper(); break;
                case CoreManager.ServerMode.Local:
                    urlHelper = new LocalUrlHelper(); break;
                default:
                    urlHelper = new Office365UrlHelper(); break;
            }

            return urlHelper.FormatPersonalUrl(serverUrl, credentials, out personalRoot);
        }
    }
}
