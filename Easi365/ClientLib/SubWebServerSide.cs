using Microsoft.SharePoint.Client;
using System;
using Generic = System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using SP = Microsoft.SharePoint.Client;
using ClientLib.Entities;
using ClientLib.Core;
using System.Threading.Tasks;

namespace ClientLib
{
    public class SubWebServerSide : ServerSide
    {
        /// <summary>
        /// 空间名称
        /// </summary>
        public string SpaceName { get; private set; }
        /// <summary>
        /// 子网站空间网站跟路径
        /// </summary>
        public string SubWebRoot { get; set; }

        protected string _document = "";

        public override string DocumentName
        {
            get
            {
                return _document;
            }
            set 
            {
                _document = value; 
            }
        }

        public SubWebServerSide(string serverRootUrl, LocalDB db, ICredentials credentials, string subWebRoot/*保留.*/)
            : base(serverRootUrl, db, credentials)
        {
            this.SubWebRoot = subWebRoot;
        }

        public override void Init()
        {
            //数据库记录日志

            //1. 初始化客户端对象模型
            this.ClientCtx = new ClientContext(_serverRoot);
            this.ClientCtx.Credentials = _credential;

            //2. 初始化下载器
            this.Downloader = new WebClient();
            this.Downloader.Credentials = _credential;

            //#下载文件出错 错误代码 403
            //http://stackoverflow.com/questions/2316111/system-net-webclient-request-gets-403-forbidden-but-browsers-do-not-with-apache

            this.Downloader.Headers["Accept"] = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
            this.Downloader.Headers["User-Agent"] = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; MDDC)";
            //下载文件时提示未授权 需要添加这个头信息
            //http://stackoverflow.com/questions/3021752/download-a-file-from-a-claims-auth-based-sharepoint-2010-site-programmatically?answertab=votes#tab-top
            this.Downloader.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");

            //3.初始化文档库
            if (DocumentName != "")
            {
                var documentName = DocumentName;
                this.RemoteLibrary = this.ClientCtx.Web.Lists.GetByTitle(documentName);
                this.ClientCtx.ExecuteQuery();

                this.ClientCtx.Load(this.RemoteLibrary,
                    lib => lib.Id,
                    lib => lib.RootFolder,
                    lib => lib.EnableMinorVersions/*创建主要版本和次要（草稿）版本*/,
                    lib => lib.EnableVersioning/*创建主要版本*/);
                this.ClientCtx.Load(this.RemoteLibrary.RootFolder, folder => folder.ServerRelativeUrl);
                this.ClientCtx.ExecuteQuery();

                // 初始化空间名称
                this.RootFolderServerRelativeUrl = this.RemoteLibrary.RootFolder.ServerRelativeUrl;
            }

            // 4.初始化当前登录用户信息
            this.ClientCtx.Load(this.ClientCtx.Web.CurrentUser,
                u => u.LoginName,
                u => u.IsSiteAdmin);
        }

        /// <summary>
        /// 获取当前用户的协作空间
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static Generic.IEnumerable<CollaborationServerSide> GetCollaborationWebsForUser(string username)
        {
            throw new NotImplementedException();
        }

        public override string DownloadFile(Entities.ServerItem si, bool needDownload, Action complete = null)
        {
            throw new NotImplementedException();
        }

        public override System.Threading.Tasks.Task<bool> ShareFileOrFolder(Entities.ServerItem si, string[] users, Entities.UserRole role,bool isLoginName = false)
        {
            throw new NotImplementedException();
        }

        public override Generic.List<Entities.ServerItem> GetItemsInFolder(string folderUrl, int folderId)
        {
            CamlQuery query = new CamlQuery();
            query.FolderServerRelativeUrl = folderUrl;
            ListItemCollection items = this.RemoteLibrary.GetItems(query);
            this.ClientCtx.Load(items, itms => itms.Include(
                    itm => itm.Id,
                    itm => itm.FileSystemObjectType,
                    itm => itm["FileLeafRef"],
                    itm => itm["FileRef"],
                    itm => itm["owshiddenversion"],
                    itm => itm["Modified"],
                    itm => itm.File.Length,
                    itm => itm["UniqueId"]
                ));
            this.ClientCtx.ExecuteQuery();

            Generic.List<ServerItem> result = new Generic.List<ServerItem>();
            foreach (ListItem itm in items)
            {
                result.Add(new ServerItem(itm, folderId, this.RootFolderServerRelativeUrl));
            }
            return result;
        }

        public override System.Threading.Tasks.Task<string> DownloadFile(ServerItem si, bool needDownload, DownloadState downloadState, Action complete = null)
        {
            throw new NotImplementedException();
        }
    }
}
