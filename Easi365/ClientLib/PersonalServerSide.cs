using ClientLib.Entities;
using System;
using Generic = System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.SharePoint.Client;
using ClientLib.SyncActions;
using ClientLib.Core;
using Microsoft.SharePoint.Client.Sharing;
using ClientLib.Utilities;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.SharePoint.Client.Social;
using ClientLib.Services;
using IO = System.IO;
using System.Diagnostics;

namespace ClientLib
{
    public class PersonalServerSide : ServerSide
    {
        const int BUFFER_SIZE = 1024;
        public string PersonalRoot { get; set; }

        //保存共享和收藏文件所在网站的上下文
        //string ： 网站地址
        Generic.Dictionary<string, ClientContext> _extensionContexts = new Generic.Dictionary<string, ClientContext>();

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

        /// <summary>
        /// Shared With Me 上下文
        /// </summary>
        //SharedWithMeContext DocumentsSharedWithMeContext;

        public PersonalServerSide(string serverRootUrl, LocalDB db, ICredentials credentials, string personalRoot)
            : base(serverRootUrl, db, credentials)
        {
            this.PersonalRoot = personalRoot;
        }

        public override void Init()
        {
            base.Init();
            
            this.RootFolderServerRelativeUrl = this.RemoteLibrary.RootFolder.ServerRelativeUrl;

            this.RemoteLibrarys = new Generic.List<List>();

            var lists = this.ClientCtx.Web.Lists;
            this.ClientCtx.Load(lists,
                ls => ls.Include(
                    s => s.Title,
                    s => s.BaseType
                    ).Where(s => s.BaseType == BaseType.DocumentLibrary));
            this.ClientCtx.ExecuteQuery();

            foreach (List lst in lists)
            {
                List list = null;
                if (DoesUserHavePermissions(lst.Title, PermissionKind.ViewListItems, out list)
                    && !list.Hidden && !list.IsApplicationList)
                    this.RemoteLibrarys.Add(list);
            }

            try
            {
                this.DocumentViewr = new ViewDocument(this);

                //this.FollowingManager = new SocialFollowingManager(this.ClientCtx);

                //this.DocumentsSharedWithMeContext = SharedWithMeContext.GetInstence(this.ClientCtx);

                //if (!this.RemoteLibrary.EnableVersioning)
                //{
                //    //查看文件库是否开启版本控制 如果没有开启 则启用
                //    this.RemoteLibrary.EnableVersioning = true;
                //    this.RemoteLibrary.Update();

                //    this.ClientCtx.ExecuteQuery();
                //}
            }
            catch (Exception ex)
            {
                Logging.Add("站点初始化失败.", ex);
                throw ex;
            }
        }

        //private Generic.List<ServerItem> _rootFolderItems;

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

        //public Generic.List<ServerItem> RootFolderItems
        //{
        //    get
        //    {
        //        if (_rootFolderItems == null)
        //            _rootFolderItems = GetItemsInFolder(this.RemoteLibrary.RootFolder.ServerRelativeUrl, 0);
        //        return _rootFolderItems;
        //    }
        //}

        public override string DownloadFile(ServerItem si, bool needDownload, Action complete = null)
        {
            if (needDownload)
            {
                CreateLocalFile createFile = new CreateLocalFile(si);
                createFile.Execute(this, false, null);
                if (complete != null)
                    complete();
                return createFile.LocalPath;
            }
            else
            {
                string localFolder = this.DB.LocalDict.ServerIdDict[si.Id].CacheFolder;
                string localPath = CoreManager.ConfigManager.Settings.PersonalFilesCache + localFolder + "\\" + si.Name;
                return localPath;
            }
            //CreateLocalFile cfile = new CreateLocalFile(si);
            //cfile.Execute(this, false, null);

            //return cfile.LocalPath;
        }

        public override async Task<string> DownloadFile(ServerItem si, bool needDownload, DownloadState downloadState, Action complete = null)
        {
            if (needDownload)
            {
                CreateLocalFile createFile = new CreateLocalFile(si);
                //文件字典中是否有当前文件的缓存 （文件版本小于服务器版本时需要下载文件，并更新本地数据库条目）
                if (this.DB.LocalDict.ServerUniqueId.ContainsKey(si.UniqueId))
                    createFile.FiRow = this.DB.LocalDict.ServerUniqueId[si.UniqueId];

                await createFile.Execute(this, downloadState);
                if (complete != null)
                    complete();
                return createFile.LocalPath;
            }
            else
            {
                string localFolder = this.DB.LocalDict.ServerIdDict[si.Id].CacheFolder;
                string localPath = CoreManager.ConfigManager.Settings.PersonalFilesCache + localFolder + "\\" + si.Name;
                return localPath;
            }
        }

        public async Task<string> DownLoadFile(string remoteUrl, string localPath, DownloadState state)
        {
            HttpWebRequest req = null;
            HttpWebResponse rsp = null;

            try
            {
                var serverItem = await GetServerItemByFileUrl(remoteUrl);

                string targetPath = localPath;

                string fileName = IO.Path.GetFileName(localPath);

                string folderName = serverItem.UniqueId;
                string folder = CoreManager.ConfigManager.Settings.PersonalFilesCache +
                   IO.Path.DirectorySeparatorChar + folderName;

                if (!IO.Directory.Exists(folder))
                    IO.Directory.CreateDirectory(folder);

                localPath = folder + IO.Path.DirectorySeparatorChar + fileName;

                state.Set();

                //创建WebRequst
                Uri uri = new Uri(remoteUrl);
                req = (HttpWebRequest)WebRequest.Create(uri);
                req.Credentials = this.ClientCtx.Credentials;
                req.Accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; MDDC)";
                //下载文件时提示未授权 需要添加这个头信息
                req.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");

                rsp = await req.GetResponseAsync(state.CancellationToken.Token) as HttpWebResponse;
                var stream = rsp.GetResponseStream();

                state.TotalBytes = rsp.ContentLength;
                byte[] buffer = new byte[BUFFER_SIZE];
                var readBytes = await stream.ReadAsync(buffer, 0, BUFFER_SIZE);

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
                            if (rsp != null)
                                rsp.Close();

                            state.Reset();
                            return string.Empty;
                        }
                    }
                }
                state.Reset();

                //IO.File.Copy(localPath, targetPath);

                //添加xml文件信息
                var fileInfo = new LocalFileInfo();
                fileInfo.Id = serverItem.UniqueId;
                fileInfo.FileName = serverItem.Name;
                fileInfo.ServerRelativePath = serverItem.ServerRelativeUrl;
                fileInfo.IsNormalFile = false;
                fileInfo.Version = serverItem.Version;

                XmlHelper.XmlSerializeToFile(fileInfo,
                    folder + "\\Info.xml", Encoding.UTF8);

                return localPath;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                req = null;
            }
        }

        /// <summary>
        /// 返回完整路径
        /// </summary>
        /// <param name="path">相对服务器路径</param>
        /// <returns></returns>
        public override string MapFullPath(string serverRelativeUrl)
        {
            return this.PersonalRoot + serverRelativeUrl;
        }

        //收藏分享文件ServerItem
        public Task<ServerItem> GetServerItemByFileUrl(string fileUrl)
        {
            return Task.Factory.StartNew<ServerItem>(() =>
                {
                    try
                    {
                        ServerItem serverItem = null;

                        var uri = new Uri(fileUrl);
                        var webUrl = uri.GetWebUriByFile();

                        ClientContext context = null;

                        if (_extensionContexts.ContainsKey(webUrl))
                            context = _extensionContexts[webUrl];
                        else
                        {
                            context = new ClientContext(webUrl);
                            context.Credentials = this.ClientCtx.Credentials;
                            _extensionContexts.Add(webUrl, context);
                        }

                        var file = context.Web.GetFileByServerRelativeUrl(uri.LocalPath);
                        context.Load(file.ListItemAllFields,
                            itm => itm.Id,
                            itm => itm.FileSystemObjectType,
                            itm => itm["FileLeafRef"],
                            itm => itm["FileRef"],
                            itm => itm["owshiddenversion"],
                            itm => itm["Modified"],
                            itm => itm.File.Length,
                            itm => itm["UniqueId"]);
                        context.ExecuteQuery();

                        serverItem = new ServerItem(file.ListItemAllFields, 0, "")
                        {
                            UniqueId = Convert.ToString(file.ListItemAllFields["UniqueId"])
                        };

                        return serverItem;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("获取文件的ServerItem错误." + fileUrl + " Error：" + ex.Message);
                        Logging.Add("获取文件的ServerItem错误." + fileUrl, ex);

                        throw ex;
                    }
                });
        }

        public override Task<ServerItem> GetServerItemByListItemAsync(string fileRelativePath, int folderId)
        {
            return Task.Factory.StartNew<ServerItem>(() =>
            {
                ServerItem serverItem = null;

                try
                {
                    File file = this.ClientCtx.Web.GetFileByServerRelativeUrl(fileRelativePath);

                    //server.ClientCtx.Load(file);
                    this.ClientCtx.Load(file.ListItemAllFields, itm => itm.Id);
                    this.ClientCtx.ExecuteQuery();
                    // if (server.ClientCtx.HasPendingRequest)
                    var listItem = this.RemoteLibrary.GetItemById(file.ListItemAllFields.Id);
                    this.ClientCtx.Load(listItem,
                            itm => itm.Id,
                            itm => itm.FileSystemObjectType,
                            itm => itm["FileLeafRef"],
                            itm => itm["FileRef"],
                            itm => itm["owshiddenversion"],
                            itm => itm["Modified"],
                            itm => itm.File.Length,
                            itm => itm["UniqueId"]);
                    this.ClientCtx.ExecuteQuery();

                    serverItem = new ServerItem(listItem, folderId, this.RootFolderServerRelativeUrl)
                   {
                       UniqueId = Convert.ToString(listItem["UniqueId"])
                   };
                }
                catch (Exception ex)
                {
                    Logging.Add("获取文件信息失败,url:" + fileRelativePath, ex);
                }

                return serverItem;
            });
        }

        public override Generic.List<ServerItem> GetItemsInFolder(string folderUrl, int folderId)
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
            foreach (ListItem listItem in items)
            {
                result.Add(new ServerItem(listItem, folderId, this.RootFolderServerRelativeUrl)
                {
                });
            }
            return result;
        }

        public override Task<bool> ShareFileOrFolder(ServerItem si, string[] users, UserRole role, bool isLoginName = false)
        {
            return Task<bool>.Factory.StartNew(() =>
            {
                try
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

                    Role userRole = Role.View;
                    switch (role)
                    {
                        case UserRole.View:
                            userRole = Role.View;
                            break;
                        case UserRole.Edit:
                            userRole = Role.Edit;
                            break;
                        case UserRole.None:
                            userRole = Role.None;
                            break;
                    }

                    var fileUrl = this.PersonalRoot + si.ServerRelativeUrl;

                    Generic.List<UserRoleAssignment> userRoleAssignments = new Generic.List<UserRoleAssignment>(users.Length);
                    for (int i = 0; i < users.Length; i++)
                    {
                        UserRoleAssignment roleAssignment = new UserRoleAssignment();
                        roleAssignment.Role = userRole;
                        if (!isLoginName)
                            roleAssignment.UserId = loginNamePrefix + users[i];
                        else
                            roleAssignment.UserId = users[i];
                        userRoleAssignments.Add(roleAssignment);
                    }

                    Generic.IList<UserSharingResult> sharedResultInfos = DocumentSharingManager.UpdateDocumentSharingInfo(
                                            context: this.ClientCtx,
                                            resourceAddress: this.PersonalRoot + si.ServerRelativeUrl,
                                            userRoleAssignments: userRoleAssignments,
                                            validateExistingPermissions: false,
                                            additiveMode: false,
                                            sendServerManagedNotification: false,
                                            customMessage: null,
                                            includeAnonymousLinksInNotification: false);
                    this.ClientCtx.ExecuteQuery();
                    return sharedResultInfos.All(result => result.Status);
                }
                catch (Exception ex)
                {
                    Logging.Add("分配权限错误:" + si.ServerRelativeUrl, ex);
                    return false;
                }
            });
        }


    }
}
