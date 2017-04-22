using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using ClientLib.Core;
using Microsoft.SharePoint.Client;

namespace ClientLib.Utilities
{
    internal abstract class UrlHelper
    {
        public abstract string FormatPersonalUrl(string rootUrl, ICredentials credentials, out string serverRoot);
    }

    internal class Office365UrlHelper : UrlHelper
    {
        //private string PERSONAL_URLFORMAT = "https://easi365-my.sharepoint.com/personal/{0}";

        public override string FormatPersonalUrl(string rootUrl, ICredentials credentials, out string serverRoot)
        {
            if(string.IsNullOrWhiteSpace(rootUrl))
                throw new ArgumentException("root url");

            Uri uri = new Uri(rootUrl);
            var scheme = uri.Scheme;
            var host = uri.Host;

            string[] hostArr = host.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            serverRoot = scheme + "://" + hostArr[0] + "-my." + hostArr[1] + "." + hostArr[2];

            SharePointOnlineCredentials cred = credentials as SharePointOnlineCredentials;
            var personalUrl = serverRoot + "/personal/" + SplitUserNameForUrl(cred.UserName);

            return personalUrl;
        }

        /// <summary>
        /// zhoutao@easi365.onmicrosoft.com 
        /// 转换为
        /// zhoutao_easi365_onmicrosoft_com
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private string SplitUserNameForUrl(string username)
        {
            var parts = username.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            //parts[1] = parts[1].Replace('.', '_');
            return string.Join("_", parts[0], parts[1].Replace('.', '_'));
        }
    }

    internal class LocalUrlHelper : UrlHelper
    {
        static string PERSONAL_URLFORMAT = "{0}/personal/{1}";

        public override string FormatPersonalUrl(string rootUrl, ICredentials credentials, out string serverRoot)
        {
            if(string.IsNullOrWhiteSpace(rootUrl))
                throw new ArgumentException("root url");

            NetworkCredential cred = credentials as NetworkCredential;

            serverRoot = CoreManager.ConfigManager.Settings.ServerUrl;

            return string.Format(PERSONAL_URLFORMAT,
                CoreManager.ConfigManager.Settings.ServerUrl, cred.UserName.GetLoginNameWithoutDomain());
        }
    }
}
