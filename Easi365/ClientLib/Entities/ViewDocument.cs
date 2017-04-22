using ClientLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Entities
{
    public class ViewDocument
    {
        string _authType = "NTLM";
        ServerSide _server;
        private readonly string EditDocPageUrlPattern = "{0}/_layouts/15/WopiFrame2.aspx?sourcedoc={1}&action=edit";

        //string ViewDocPageUrlPattern = "{0}/_layouts/15/WopiFrame.aspx?sourcedoc={1}&action=embedview";
        private readonly string ViewDocPageUrlPattern = "{0}/_layouts/15/WopiFrame.aspx?sourcedoc={1}&action=View";

        //office在线查看链接格式 1 条目GUID  2 文件名称 3 查看类型 edit view
        static string _office365ViewDocPageUrlPattern = "{0}/_layouts/15/WopiFrame.aspx?sourcedoc={1}&file={2}&action={3}";

        private static string _office365EmbedviewUrlPattern = "{0}/_layouts/15/WopiFrame.aspx?sourcedoc={1}&action=embedview";

        public static string Office365ViewDocPageUrlPattern
        {
            get
            {
                return _office365ViewDocPageUrlPattern;
            }
        }

        public static string Office365EmbedviewUrlPattern
        {
            get { return _office365EmbedviewUrlPattern; }
        }
        
        public ViewDocument(ServerSide server)
        {
            _server = server;
        }

        private async Task<string> GetDocContentAsync(Uri uri)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

                CredentialCache cc = new CredentialCache();
                cc.Add(uri, _authType, _server.ClientCtx.Credentials as NetworkCredential);
                request.Credentials = cc;
                request.AllowAutoRedirect = true;

                HttpWebResponse rsp = await request.GetResponseAsync() as HttpWebResponse;
                var stream = rsp.GetResponseStream();

                using (StreamReader reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                Logging.Add("文档预览失败", ex);
                return "<html><head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8' /></head><body style='text-align:center; margin-top: 150px; font-size:12px;'>没有预览</body></html>";
            }
        }

        public async Task<string> GetViewDocContentAsync(string relativePath)
        {
            Uri uri =  new Uri(string.Format(ViewDocPageUrlPattern, _server.ClientCtx.Url, relativePath));
            return await GetDocContentAsync(uri);
        }

        public async Task<string> GetEditDocContentAsync(string relativePath)
        {
            Uri uri = new Uri(string.Format(EditDocPageUrlPattern, _server.ClientCtx.Url, relativePath));
            return await GetDocContentAsync(uri);
        }
    }
}
