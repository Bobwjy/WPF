using System;
using Generic = System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using ClientLib.Utilities;
using ClientLib.Core;

namespace ClientLib
{
    public class CollaborationServerManager
    {
        SyncManager _sm;
        ICredentials _credentials;
        public Generic.Dictionary<string, ServerSide> CollaborationServers { get; private set; }

        public CollaborationServerManager(SyncManager sm, ICredentials credentials)
        {
            _sm = sm;
            _credentials = credentials;
            CollaborationServers = new Generic.Dictionary<string, ServerSide>();
        }

        /// <summary>
        /// 创建协作空间
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        public void CreateCollaborationSpace(string url, string title, string description)
        {
        }

        /// <summary>
        /// 删除协作空间
        /// </summary>
        /// <param name="guid"></param>
        public void DeleteCollaboration(string guid)
        { }

        /// <summary>
        /// 查找协作空间
        /// </summary>
        /// <param name="guid">网站ID</param>
        /// <returns></returns>
        public ServerSide FindCollaborationSpace(string guid, CoreManager.SpaceCategory sc)
        {
            if (CollaborationServers.ContainsKey(guid))
                return CollaborationServers[guid];

            string webName = "";
            if (sc == CoreManager.SpaceCategory.CooSpace) 
                webName = Constants.SpaceSite.CooSpace;
            else 
                webName = Constants.SpaceSite.Depeartment; 

            //查找协作空间
            var rootUrl = _sm.CompanyServerSide.ClientCtx.Url + "/" + webName + "/" + guid;

            CollaborationServerSide server = new CollaborationServerSide(rootUrl, _sm.DB, _credentials, rootUrl);
            server.Init();
            CollaborationServers[guid] = server;

            return CollaborationServers[guid];
        }
    }
}
