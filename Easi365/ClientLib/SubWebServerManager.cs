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
    public class SubWebServerManager
    {
        SyncManager _sm;
        ICredentials _credentials;
        public Generic.Dictionary<string, ServerSide> SubWebServers { get; private set; }

        public SubWebServerManager(SyncManager sm, ICredentials credentials)
        {
            _sm = sm;
            _credentials = credentials;
            SubWebServers = new Generic.Dictionary<string, ServerSide>();
        }

        /// <summary>
        /// 查找子网站空间
        /// </summary>
        /// <param name="rootUrl">网站Url</param>
        /// <returns></returns>
        public ServerSide FindSubWebSpace(string rootUrl, string document = "")
        {
            if (SubWebServers.ContainsKey(rootUrl))
                return SubWebServers[rootUrl];

            SubWebServerSide server = new SubWebServerSide(rootUrl, _sm.DB, _credentials, rootUrl);
            if (document != "") server.DocumentName = document;
            server.Init();
            SubWebServers[rootUrl] = server;

            return SubWebServers[rootUrl];
        }
    }
}
