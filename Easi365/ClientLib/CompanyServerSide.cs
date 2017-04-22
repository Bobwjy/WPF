using Microsoft.SharePoint.Client;
using System;
using Generic = System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using ClientLib.Entities;
using ClientLib.SyncActions;
using System.Threading.Tasks;
using IO = System.IO;
using ClientLib.Core;

namespace ClientLib
{
    public class CompanyServerSide : ServerSide
    {
        const int BUFFER_SIZE = 1024;

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

        public string CompanyRoot { get; set; }

        public CompanyServerSide(string serverRootUrl, LocalDB db, ICredentials credentials, string companyRoot)
            : base(serverRootUrl, db, credentials)
        {
            this.CompanyRoot = companyRoot;
        }

        public override void Init()
        {
            base.Init();

            this.RemoteLibrarys = new Generic.List<List>();

            var lists = this.ClientCtx.Web.Lists;
            //this.ClientCtx.Load(lists,
            //    ls => ls.Include(
            //        s => s.Title,
            //        s => s.BaseType,
            //        s => s.EffectiveBasePermissions,
            //        s => s.RootFolder.ServerRelativeUrl,
            //        s => s.IsSiteAssetsLibrary,
            //        s => s.IsApplicationList,
            //        s => s.Hidden,
            //        s => s.ParentWeb.Url
            //        ).Where(s => s.BaseType == BaseType.DocumentLibrary));
            this.ClientCtx.Load(lists,
                ls => ls.Include(
                    s => s.Title,
                    s => s.BaseType
                    ).Where(s => s.BaseType == BaseType.DocumentLibrary));
            this.ClientCtx.ExecuteQuery();

            foreach (List lst in lists)
            {
                List list = null;
                //if (!lst.Hidden && !lst.IsApplicationList && lst.EffectiveBasePermissions.Has(PermissionKind.ViewListItems))
                //    this.RemoteLibrarys.Add(lst);
                if (DoesUserHavePermissions(lst.Title, PermissionKind.ViewListItems, out list)
                    && !list.Hidden && !list.IsApplicationList)
                    this.RemoteLibrarys.Add(list);
            }

            this.RootFolderServerRelativeUrl = this.RemoteLibrary.RootFolder.ServerRelativeUrl;
        }

        private bool DoesUserHavePermissions(string listName, PermissionKind permmask, out List list)
        {
            try
            {
                List targetList = this.ClientCtx.Web.Lists.GetByTitle(listName);
                this.ClientCtx.Load(targetList,
                    s => s.Title,
                    s => s.EffectiveBasePermissions,
                    s => s.RootFolder,
                    s => s.IsSiteAssetsLibrary,
                    s => s.BaseType,
                    s => s.IsApplicationList,
                    s => s.Hidden,
                    s => s.ParentWeb.Url);
                this.ClientCtx.Load(targetList.RootFolder, folder => folder.ServerRelativeUrl);
                this.ClientCtx.ExecuteQuery();

                list = targetList;

                return targetList.EffectiveBasePermissions.Has(permmask);
            }
            catch (Exception ex)
            {
                Logging.Add("读取列表权限失败", ex);
                list = null;
                return false;
            }
        }

        public override string DownloadFile(Entities.ServerItem si, bool needDownload, Action complete = null)
        {
            throw new NotImplementedException();
        }

        public void CreateNewFile(ClientItem ci, ServerItem parentSi, string path, Action<bool> uploaded)
        {
            CreateRemoteFile createFile = new CreateRemoteFile(ci, path);
            if (parentSi != null)
                createFile.ServerItem = parentSi;

            createFile.Execute(this, false, uploaded);
        }

        /// <summary>
        /// 返回完整路径
        /// </summary>
        /// <param name="path">相对服务器路径</param>
        /// <returns></returns>
        public override string MapFullPath(string serverRelativeUrl)
        {
            return this.CompanyRoot + serverRelativeUrl;
        }

        public override System.Threading.Tasks.Task<bool> ShareFileOrFolder(Entities.ServerItem si, string[] users, Entities.UserRole role, bool isLoginName = false)
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

        public async Task DownLoadFile(string remoteUrl, string localPath, DownloadState state)
        {
            try
            {
                state.Set();

                //创建WebRequst
                Uri uri = new Uri(remoteUrl);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                req.Credentials = this.ClientCtx.Credentials;
                req.Accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; MDDC)";
                //下载文件时提示未授权 需要添加这个头信息
                req.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");

                var rsp = await req.GetResponseAsync() as HttpWebResponse;
                var stream = rsp.GetResponseStream();

                state.TotalBytes = rsp.ContentLength;
                byte[] buffer = new byte[BUFFER_SIZE];
                var readBytes = await stream.ReadAsync(buffer, 0, BUFFER_SIZE);

                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(localPath)))
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(localPath));

                using (System.IO.FileStream fs = new IO.FileStream(localPath, IO.FileMode.OpenOrCreate))
                {
                    while (readBytes > 0)
                    {
                        await fs.WriteAsync(buffer, 0, readBytes);
                        state.BytesRead += readBytes;
                        var percent = ((double)state.BytesRead / (double)state.TotalBytes);
                        state.DownloadPercent = percent;
                        readBytes = await stream.ReadAsync(buffer, 0, BUFFER_SIZE, state.CancellationToken.Token);

                        if (state.CancellationToken.IsCancellationRequested)
                        {
                            //取消下载任务后 删除没有下载完成的文件
                            state.Reset();
                            return;
                        }
                    }
                }

                state.Reset();

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
