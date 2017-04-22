using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ClientLib.Core;
using ClientLib.Entities;
using Microsoft.SharePoint.Client;
using System.Text.RegularExpressions;

namespace ClientLib.Utilities
{
    public static class Common
    {
        public static string GetLoginNameWithoutDomain(this string loginName)
        {
            return loginName.Substring(loginName.LastIndexOf('\\') + 1);
        }

        public static string GetDomain(this string loginName)
        {
            if (loginName.LastIndexOf('\\') > 0)
                return loginName.Substring(0, loginName.LastIndexOf('\\'));
            return loginName;
        }

        public static User GetSiteUserById(ClientContext context, int id)
        {
            try
            {
                var user = context.Web.SiteUsers.GetById(id);
                context.Load(user);
                context.ExecuteQuery();

                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool IsSiteUser(ClientContext context, string loginName)
        {
            try
            {
                var user = context.Web.EnsureUser(loginName);
                context.ExecuteQuery();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static FieldUserValue[] GetUserValues(ClientContext clientContext, IList<CheckedUser> users)
        {
            var siteUsers = users.Where(m => IsSiteUser(clientContext, m.Account)).ToArray<CheckedUser>();
            if (siteUsers.Length > 0)
            {
                var arrUsers = new FieldUserValue[siteUsers.Length];
                for (int i = 0; i < arrUsers.Length; i++)
                {
                    arrUsers[i] = FieldUserValue.FromUser(siteUsers[i].Account);
                }

                return arrUsers;
            }

            return null;
        }

        public static FieldUserValue[] GetUserValues(string siteUrl, ICredentials credentials, IList<CheckedUser> users)
        {
            var clientContext = new ClientContext(CoreManager.ConfigManager.Settings.ServerUrl);
            clientContext.Credentials = credentials;
            var siteUsers = users.Where(m => IsSiteUser(clientContext, m.Account)).ToArray<CheckedUser>();
            if (siteUsers.Length > 0)
            {
                var arrUsers = new FieldUserValue[siteUsers.Length];
                for (int i = 0; i < arrUsers.Length; i++)
                {
                    arrUsers[i] = FieldUserValue.FromUser(siteUsers[i].Account);
                }

                return arrUsers;
            }

            return null;
        }

        public static User GetUserByFieldUserValue(ClientContext clientContext,FieldUserValue userValue)
        {
            try
            {
                User user = null;
                if (ClientLib.Utilities.Common.IsSiteUser(clientContext, userValue.LookupValue))
                {
                    var ensureUser = clientContext.Web.EnsureUser(userValue.LookupValue);
                    clientContext.Load(ensureUser);
                    clientContext.ExecuteQuery();

                    user = clientContext.Web.EnsureUser(ensureUser.LoginName.GetLoginNameWithoutDomain());
                }
                return user;
            }
            catch { return null; }
        }

        ///   <summary>   
        ///   去除HTML标记   
        ///   </summary>   
        ///   <param   name="NoHTML">包括HTML的源码   </param>   
        ///   <returns>已经去除后的文字</returns>   
        public static string NoHTML(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([/r/n])[/s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "/", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "/xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "/xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "/xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "/xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(/d+);", "", RegexOptions.IgnoreCase);

            Htmlstring = Htmlstring.Replace("<", "");
            Htmlstring = Htmlstring.Replace(">", "");
            Htmlstring = Htmlstring.Replace(System.Environment.NewLine, string.Empty);

            return Htmlstring;
        }
    }
}
