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
    public class CollaborationServerSide : ServerSide
    {
        /// <summary>
        /// 空间名称
        /// </summary>
        public string SpaceName { get; private set; }
        /// <summary>
        /// 协作空间网站跟路径
        /// </summary>
        public string CollaborationRoot { get; set; }

        protected string _document = "文档";

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

        //public CollaborationServerSide(string serverRootUrl, LocalDB db, string userName, string password)
        //    : base(serverRootUrl, db, userName, password)
        //{

        //}

        public CollaborationServerSide(string serverRootUrl, LocalDB db, ICredentials credentials, string collaborationRoot/*保留.*/)
            : base(serverRootUrl, db, credentials)
        {
            this.CollaborationRoot = collaborationRoot;
        }

        public override void Init()
        {
            base.Init();

            // 初始化空间名称
            //this.SpaceName = this.ClientCtx.Web.Title;
            this.RootFolderServerRelativeUrl = this.RemoteLibrary.RootFolder.ServerRelativeUrl;
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
