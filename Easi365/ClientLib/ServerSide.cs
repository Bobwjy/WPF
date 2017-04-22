using System;
using Generic = System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using System.Net;
using System.Security;
using ClientLib.Services;
using ClientLib.Entities;
using ClientLib.SyncActions;
using ClientLib.Core;
using IO = System.IO;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client.Sharing;
using ClientLib.Utilities;
using Microsoft.SharePoint.Client.Social;
using System.Collections.Generic;
using ClientLib.SyncLogService;
using Microsoft.SharePoint.Client.Utilities;

namespace ClientLib
{
    public abstract class ServerSide
    {
        //private const string LIBRARY_NAME = "easy365";
        //private const string LIBRARY_NAME = "文档";


        protected ICredentials _credential;
        //protected ICredentials _credential;
        protected string _serverRoot;
        protected LocalDB _db;

        /// <summary>
        /// 服务器网站的地址
        /// </summary>
        public virtual string ServerRoot
        {
            get
            {
                return _serverRoot;
            }
        }

        /// <summary>
        /// 本地数据库
        /// </summary>
        public LocalDB DB { get { return _db; } }
        /// <summary>
        /// SharePoint客户端对象模型上下文
        /// </summary>
        public ClientContext ClientCtx { get; protected set; }
        /// <summary>
        /// 文件下载器
        /// </summary>
        public WebClient Downloader { get; protected set; }
        /// <summary>
        /// 文档库
        /// </summary>
        public List RemoteLibrary { get; set; }
        /// <summary>
        /// 文档库集合
        /// </summary>
        public Generic.List<List> RemoteLibrarys { get; set; }

        /// <summary>
        /// 文档库服务器相对路径
        /// </summary>
        public string RootFolderServerRelativeUrl { get; set; }
        /// <summary>
        /// 当前登录用户
        /// </summary>
        public UserInfo User { get; set; }

        /// <summary>
        /// 网盘文档库
        /// </summary>
        public abstract string DocumentName { get; set; }

        public PeopleManager UPSManager { get; protected set; }

        /// <summary>
        /// 文档预览器
        /// </summary>
        public ViewDocument DocumentViewr { get; protected set; }

        /// <summary>
        /// 文档关注管理器
        /// </summary>
        public SocialFollowingManager FollowingManager { get; protected set; }

        private PersonProperties _profileProperties;
        public UserInfo GetProfileProperties(Action<string> done)
        {

            if (this._profileProperties == null)
            {
                this.User = new UserInfo();
                this.UPSManager = new PeopleManager(this.ClientCtx);

                this.User = new UserInfo();
                //this._profileProperties = this.UPSManager.GetPropertiesFor(
                //    string.Format(CoreManager.LoginNamePrefix, CoreManager.ConfigManager.Settings.CurrentUserName));
                this._profileProperties = this.UPSManager.GetPropertiesFor(this.ClientCtx.Web.CurrentUser.LoginName);
                //this.ClientCtx.Load(this._profileProperties,
                //    p => p.PictureUrl,
                //    p => p.AccountName,
                //    p => p.DisplayName,
                //    p => p.Title);
                this.ClientCtx.Load(this._profileProperties);
                this.ClientCtx.ExecuteQuery();

                this.User.PictureUrl = this._profileProperties.PictureUrl;
                this.User.Account = this._profileProperties.AccountName;
                this.User.DisplayName = this._profileProperties.DisplayName;

                //下载用户头像
                //if (this.User.PictureUrl != null)
                //{
                //    var t = new System.Threading.Thread(() =>
                //    {
                //        SaveTheUserPictureToLocal(this.User.PictureUrl, done);
                //    });
                //    t.IsBackground = true;
                //    t.Start();
                //}
            }
            return this.User;
        }

        /// <summary>
        /// https://easi365-my.sharepoint.com:443/xxxxxxxx?t=63531343616
        /// 检查用户图像文件是否过期
        /// </summary>
        /// <param name="url"></param>
        /// <returns>图片是否</returns>
        private bool CheckTimestampOverdue(string url, string localPath, out string timeStamp)
        {
            Generic.Dictionary<string, string> querys = new Generic.Dictionary<string, string>();
            string pictureTimeStamp = CoreManager.ConfigManager.Settings.MThumbTimeStamp;
            timeStamp = CoreManager.ConfigManager.Settings.MThumbTimeStamp;
            bool overdue = true;

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(localPath);
            if (!fileInfo.Exists)
                return true;

            Uri uri = new Uri(url);

            if (!string.IsNullOrEmpty(uri.Query))
            {
                var query = uri.Query.Substring(1);
                var pairs = query.Split(new char[] { '&' });
                for (int i = 0; i < pairs.Length; i++)
                {
                    var keyValue = pairs[0].Split(new char[] { '=' });
                    if (!querys.ContainsKey(keyValue[0]))
                        querys.Add(keyValue[0], keyValue[1]);
                    if (keyValue[0] == "t")
                        break;
                }

                if (!querys.ContainsKey("t"))
                    return true;

                timeStamp = querys["t"];
                return querys["t"] != pictureTimeStamp;
            }

            return true;
        }

        //private void SaveTheUserPictureToLocal(string url, Action<string> done)
        //{
        //    if (string.IsNullOrEmpty(url))
        //        throw new ArgumentException("UserPicture");

        //    try
        //    {
        //        string localPath = CoreManager.ConfigManager.Settings.PersonalFilesFolder + "\\MThumb.jpg";
        //        string timeStamp;
        //        bool picOverdue = CheckTimestampOverdue(url, localPath, out timeStamp);

        //        if (picOverdue)
        //        {
        //            //this.Downloader.DownloadFile(url, localPath);
        //            var imgData = this.Downloader.DownloadData(url);
        //            System.IO.File.WriteAllBytes(localPath, imgData);
        //            CoreManager.ConfigManager.Settings.MThumbTimeStamp = timeStamp;
        //            CoreManager.ConfigManager.SaveSettings();
        //        }
        //        done(localPath);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.Add("下载用户头像错误", ex);
        //    }
        //}


        public ServerSide(string serverUrl, LocalDB db)
        {
            _serverRoot = serverUrl.TrimEnd('/');
            _db = db;
        }

        public ServerSide(string serverRootUrl, LocalDB db, string userName, string password)
            : this(serverRootUrl, db)
        {
            _credential = new SharePointOnlineCredentials(userName, GenSecurityPassword(password));
        }

        public ServerSide(string serverRootUrl, LocalDB db, ICredentials credentials)
            : this(serverRootUrl, db)
        {
            _credential = credentials;

        }

        public virtual void Init()
        {
            try
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

                //3. 获取文档库信息
                //this.RemoteLibrary = this.ClientCtx.Web.Lists.GetByTitle(DocumentName);
                //this.ClientCtx.Load(this.RemoteLibrary, lib => lib.Id, lib => lib.RootFolder);
                //this.ClientCtx.Load(this.RemoteLibrary.RootFolder, folder => folder.ServerRelativeUrl);
                //this.ClientCtx.ExecuteQuery();

                //this.RootFolderServerRelativeUrl = this.RemoteLibrary.RootFolder.ServerRelativeUrl;

                //初始化userprofile管理类

                //3.初始化文档库
                var documentName = DocumentName;
                this.RemoteLibrary = this.ClientCtx.Web.Lists.GetByTitle(documentName);
                this.ClientCtx.ExecuteQuery();



                //var listCollection = this.ClientCtx.Web.Lists;
                //this.ClientCtx.Load(listCollection, 
                //    lists => 
                //        lists.Include(list => list.Title)
                //        .Where(list => list.Title == documentName));
                //this.ClientCtx.ExecuteQuery();

                //if (listCollection.Count > 0)
                //{
                //    //this.RemoteLibrary = this.ClientCtx.Web.Lists.GetByTitle(DocumentName);
                //    //this.ClientCtx.ExecuteQuery();
                //    this.RemoteLibrary = listCollection[0];
                //}
                //else
                //{
                //    Web web = this.ClientCtx.Web;
                //    ListCollection collList = this.ClientCtx.Web.Lists;

                //    ListCreationInformation lci = new ListCreationInformation();
                //    lci.Title = DocumentName;
                //    lci.TemplateType = (int)ListTemplateType.DocumentLibrary;
                //    List doc = web.Lists.Add(lci);

                //    this.ClientCtx.Load(collList);
                //    this.ClientCtx.ExecuteQuery();

                //    this.RemoteLibrary = doc;
                //}

                // 4.初始化当前登录用户信息
                this.ClientCtx.Load(this.ClientCtx.Web.CurrentUser,
                    u => u.LoginName,
                    u => u.IsSiteAdmin);

               
                this.ClientCtx.Load(this.RemoteLibrary, 
                    lib => lib.Id, 
                    lib => lib.RootFolder,
                    lib => lib.EnableMinorVersions/*创建主要版本和次要（草稿）版本*/,
                    lib => lib.EnableVersioning/*创建主要版本*/);
                this.ClientCtx.Load(this.RemoteLibrary.RootFolder, folder => folder.ServerRelativeUrl);
                this.ClientCtx.ExecuteQuery();
            }
            catch (Exception ex)
            {
                Logging.Add("113 - 服务器初始化异常", ex);
                throw;
            }
        }

        /// <summary>
        /// 新建文件夹
        /// </summary>
        /// <param name="parentSi"></param>
        /// <param name="folderUrl"></param>
        /// <param name="complete"></param>
        public async Task<ServerItem> CreateNewFolder(ServerItem parentSi, string folderUrl, Action<bool> complete = null)
        {
            CreateRemoteFolder folderCreation = new CreateRemoteFolder(null, folderUrl);
            if (parentSi != null)
                folderCreation.ServerItem = parentSi;

            return await folderCreation.Execute(this, false);
        }

        /// <summary>
        /// 新建文件
        /// </summary>
        /// <param name="ci"></param>
        /// <param name="parentSi"></param>
        /// <param name="path"></param>
        /// <param name="uploaded"></param>
        public void CreateNewFile(ClientItem ci, ServerItem parentSi, string path, Action<bool> uploaded)
        {
            CreateRemoteFile createFile = new CreateRemoteFile(ci, path);
            if (parentSi != null)
                createFile.ServerItem = parentSi;

            createFile.Execute(this, false, uploaded);
        }

        public async Task CreateNewFileAsync(ClientItem ci, ServerItem parentSi, UploadState state, string path, Action<bool, ServerItem> uploaded)
        {
            CreateRemoteFile createFile = new CreateRemoteFile(ci, path);
            if (parentSi != null)
                createFile.ServerItem = parentSi;

            await createFile.Execute(this, state, uploaded);
        }

        public Task DeleteFolder(ServerItem si, string path)
        {
            //var fiRow = this.DB.LocalDict.ServerIdDict[si.Id];
            return Task.Factory.StartNew(()=>
            {
                DeleteRemoteFolder delFolder = new DeleteRemoteFolder(si, path);
                delFolder.Execute(this, false, null);
            });
        }

        public Task DeleteFile(ServerItem si, string path)
        {
            //var fiRow = this.DB.LocalDict.ServerIdDict[si.Id];
            return Task.Factory.StartNew(() => 
            {
                DeleteRemoteFile delFile = new DeleteRemoteFile(si, path);
                delFile.Execute(this, false, null);
            });
        }

        /// <summary>
        /// 返回完整路径
        /// </summary>
        /// <param name="path">相对服务器路径</param>
        /// <returns></returns>
        public virtual string MapFullPath(string serverRelativeUrl)
        {
            return this.ServerRoot + serverRelativeUrl;
        }

        /// <summary>
        /// 检查是否需要下载服务器端文件
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        public bool CheckIfFileNeedDownload(ServerItem si,Func<bool> confirm)
        {
            //本地数据库中没有该文件的信息 需要重新下载文件
            if (!this.DB.LocalDict.ServerUniqueId.ContainsKey(si.UniqueId))
                return true;

            var fileRow = this.DB.LocalDict.ServerUniqueId[si.UniqueId];

            if (fileRow.IsCacheFolderNull())
                return true;

            if (fileRow.IsServerVersionNull())
                return true;

            //var folderName = fileRow.CacheFolder;
            //var fileName = fileRow.ItemName;
            IO.FileInfo fileinfo = new IO.FileInfo(CoreManager.ConfigManager.Settings.PersonalFilesCache +
                fileRow.CacheFolder + "\\" +
                fileRow.ItemName);

            if (!fileinfo.Exists)
                return true;

            if (fileinfo.Length != si.FileSize)
                return true;

            int siVersion = GetServerItemVersion(si.Id);

            //如果本地版本和服务器版本不同 并且本地版本小于服务器版本 需要重新下载
            //当有新版本时 让用户确认是否更新文件
            var localVersion = fileRow.ServerVersion;
            if (localVersion != siVersion
                && localVersion < siVersion)
            {
                return confirm();
            }

            return false;
        }

        private int GetServerItemVersion(int id)
        {
            ListItem item = this.RemoteLibrary.GetItemById(id);

            this.ClientCtx.Load(item,
                itm => itm["owshiddenversion"]);
            this.ClientCtx.ExecuteQuery();

            return Convert.ToInt32(item["owshiddenversion"]);
        }

        public ServerItem ModifyFileName(int id, string newName)
        {
            // if (server.ClientCtx.HasPendingRequest)
            var listItem = this.RemoteLibrary.GetItemById(id);
            //listItem["Name"] = newName;
            //listItem.Update();
            this.ClientCtx.Load(listItem,
                itm => itm["FileDirRef"],
                itm => itm.FileSystemObjectType,
                itm => itm["FileRef"]);
            this.ClientCtx.ExecuteQuery();

            if (listItem.FileSystemObjectType == FileSystemObjectType.File)
            {
                string newPath = listItem["FileDirRef"] + "/" + newName;
                listItem.File.MoveTo(newPath, MoveOperations.Overwrite);
                //listItem.Update();
                this.ClientCtx.ExecuteQuery();
            }
            else
            {
                listItem["FileLeafRef"] = newName;
                listItem.Update();
                this.ClientCtx.ExecuteQuery();
            }

            //string newPath = files[0]["FileDirRef"] + "/" + "MyNewFileName" + ".extension";
            //files[0].File.MoveTo(newPath, MoveOperations.Overwrite);
            //files[0].Update();
            //clientContext.ExecuteQuery();

            this.ClientCtx.Load(listItem,
                    itm => itm.Id,
                    itm => itm.FileSystemObjectType,
                    itm => itm["FileLeafRef"],
                    itm => itm["FileRef"],
                    itm => itm["owshiddenversion"],
                    itm => itm["Modified"],
                    itm => itm.File.Length,
                    itm => itm["SharedWith"]);
            this.ClientCtx.ExecuteQuery();

            var serverItem = new ServerItem(listItem, 0, this.RootFolderServerRelativeUrl)
            {
                SharedWith = Convert.ToInt32(listItem["SharedWith"])
            };

            return serverItem;
        }

        // 0 服务器地址
        // 1 列表id {xxxx-xxx-xxx-xx}
        // 2 文件的服务器相对路径
        // 3 服务器地址
        private readonly string versionHistoryFormat = "{0}/_layouts/15/Versions.aspx?list={1}&ID={2}&FileName={3}&Source={4}%2FDocuments%2FForms%2FAll.aspx&IsDlg=1";
        string ssolink = "{0}/_layouts/15/sso/ClientSSO.aspx?userToken={1}&password={2}&jmp={3}";

        public string GetHistoryUrl(ServerItem si,string username,string password)
        {
            string  jmp = string.Format(versionHistoryFormat,
                this.ClientCtx.Url,
                string.Format("{{{0}}}", this.RemoteLibrary.Id),
                si.Id,
                HttpUtility.UrlKeyValueEncode(si.ServerRelativeUrl),
                 HttpUtility.UrlKeyValueEncode(this.ClientCtx.Url));
           
            string sso = string.Format(ssolink, this.ClientCtx.Url, username, password,
                HttpUtility.UrlKeyValueEncode(jmp)
                );

            return sso;
        }

        private readonly string recycleBinUrl = "{0}/_layouts/15/RecycleBin.aspx?IsDlg=1";
        /// <summary>
        /// 获取当前网站回收站URL
        /// </summary>
        /// <returns></returns>
        public string GetRecycleBinUrl(string username,string password)
        {
            return string.Format(recycleBinUrl,
                this.ClientCtx.Url);
        }

        /// <summary>
        /// 获取当前文档的分享用户
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        public async Task<Generic.List<SharedUserModel>> GetSharingInformation(ServerItem si)
        {
            return await Task<Generic.List<SharedUserModel>>.Factory.StartNew(() =>
            {
                try
                {
                    Generic.List<SharedUserModel> results = new Generic.List<SharedUserModel>();

                    ListItem item = this.RemoteLibrary.GetItemById(si.Id);

                    ObjectSharingInformation sharingInfo = ObjectSharingInformation.GetObjectSharingInformation(
                                    this.ClientCtx,
                                    item,
                                    true, true, false, true, true, true, true);
                    var sharedUsers = sharingInfo.GetSharedWithUsers();

                    this.ClientCtx.Load(sharedUsers, users => users.Include(
                        usr => usr.Name,
                        usr => usr.HasEditPermission,
                        usr => usr.HasViewPermission,
                        usr => usr.LoginName
                        ));
                    this.ClientCtx.ExecuteQuery();

                    foreach (ObjectSharingInformationUser usr in sharedUsers)
                    {
                        SharedUserModel sharedUserModel = new SharedUserModel();
                        sharedUserModel.UserName = usr.Name;
                        sharedUserModel.UserRole = usr.HasEditPermission ? UserRole.Edit : UserRole.View;
                        sharedUserModel.LoginName = usr.LoginName;
                        results.Add(sharedUserModel);
                    }

                    return results;
                }
                catch (Exception ex)
                {
                    Logging.Add("查询分享用户失败." + si.ServerRelativeUrl, ex);
                    throw ex;
                }
            });
        }

        public abstract Task<bool> ShareFileOrFolder(ServerItem si, string[] users, UserRole role,bool isLoginName = false);
        public abstract string DownloadFile(ServerItem si, bool needDownload, Action complete = null);

        /// <summary>
        /// 获取当前文档库某目录中的所有条目
        /// </summary>
        /// <param name="folderUrl"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public abstract Generic.List<ServerItem> GetItemsInFolder(string folderUrl, int folderId);



        public abstract Task<string> DownloadFile(ServerItem si, bool needDownload, DownloadState downloadState, Action complete = null);
        #region 密码加密
        private SecureString GenSecurityPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password");

            SecureString pwd = new SecureString();
            foreach (char c in password)
                pwd.AppendChar(c);

            return pwd;
        }
        #endregion

        public Task<Generic.List<Entities.ServerItem>> GetFolderInFolder(string folderUrl, int folderId)
        {
            return Task<Generic.List<Entities.ServerItem>>.Factory.StartNew(() =>
            {
                try
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
                        if (itm.FileSystemObjectType == FileSystemObjectType.File) continue;
                        result.Add(new ServerItem(itm, folderId, this.RootFolderServerRelativeUrl));
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    Logging.Add("加载空间文件夹报错", ex);
                    return null;
                }
            });
        }

        /// <summary>
        /// 文档的关注与取消关注
        /// </summary>
        /// <param name="followUrl"></param>
        /// <param name="followUrl">isFollow</param>
        public void DocumentFollowing(string followUrl, bool isFollow)
        {
            SocialActorInfo actorInfo = new SocialActorInfo();
            actorInfo.ContentUri = followUrl;
            actorInfo.ActorType = SocialActorType.Document;

            ClientResult<bool> isFollowed = this.FollowingManager.IsFollowed(actorInfo);
            this.ClientCtx.ExecuteQuery();

            if (isFollowed.Value && !isFollow)
            {
                this.FollowingManager.StopFollowing(actorInfo);
            }
            else
            {
                this.FollowingManager.Follow(actorInfo);
            }
            this.ClientCtx.ExecuteQuery();
        }

        public Task<bool> HasFollowed(string followUrl)
        {
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    SocialActorInfo actorInfo = new SocialActorInfo();
                    actorInfo.ContentUri = followUrl;
                    actorInfo.ActorType = SocialActorType.Document;

                    ClientResult<bool> isFollowed = this.FollowingManager.IsFollowed(actorInfo);
                    this.ClientCtx.ExecuteQuery();

                    return isFollowed.Value;
                }
                catch (Exception ex)
                {
                    Logging.Add("判断文档是否被关注异常", ex);
                    return false;
                }
            });
        }

        public void CheckFileIfCached(ServerItem si, Action<bool> action)
        {
            if (si.ItemType == FileOrFolderType.Folder)
                return;

            Task.Factory.StartNew(delegate()
            {
                if (DB.LocalDict.ServerUniqueId.ContainsKey(si.UniqueId))
                {
                    if (!DB.LocalDict.ServerUniqueId[si.UniqueId].IsCacheFolderNull())
                    {
                        var cacheFolder = DB.LocalDict.ServerIdDict[si.Id].CacheFolder;
                        string localPath = CoreManager.ConfigManager.Settings.PersonalFilesCache + cacheFolder + "\\" + si.Name;

                        System.IO.FileInfo fileInfo = new IO.FileInfo(localPath);
                        if (action != null)
                            action(fileInfo.Exists);
                    }
                }
            });
        }

        /// <summary>
        /// 文件上传完毕后获取文件信息 
        /// </summary>
        /// <param name="fileRelativePath"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public virtual Task<ServerItem> GetServerItemByListItemAsync(string fileRelativePath, int folderId) 
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
                            itm => itm.File.Length);
                    this.ClientCtx.ExecuteQuery();

                    serverItem = new ServerItem(listItem, folderId, this.RootFolderServerRelativeUrl);

                }
                catch (Exception ex)
                {
                    Logging.Add("获取文件信息失败,url:" + fileRelativePath, ex);
                }

                return serverItem;
            });
        }

        public Generic.List<ServerItem> SearchItems(string content, int folderId, string folderUrl)
        {
            //var li = this.ClientCtx.Web.Lists.GetByTitle(_document);
            CamlQuery query = new CamlQuery();
            query.FolderServerRelativeUrl = folderUrl;
            query.ViewXml = "<View Scope='Recursive'><Query><Where><Contains><FieldRef Name='FileLeafRef' /><Value Type='Text'>" + content + "</Value></Contains></Where></Query></View>";
            var items = this.RemoteLibrary.GetItems(query);
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
                    //SharedWith = Convert.ToInt32(listItem["PrincipalCount"])
                });
            }

            return result;
        }

        public void FilePaste(string Url, string serverRelPath, bool overwrite)
        {
            try
            {
                var space = Url.Split(new char[] { '/' })[1];
                var library = Url.Split(new char[] { '/' })[2];

                FileInformation fileinfo = File.OpenBinaryDirect(this.ClientCtx, Url);

                File.SaveBinaryDirect(this.ClientCtx, serverRelPath, fileinfo.Stream, true);
            }
            catch (Exception ex)
            {
                Logging.Add("读取列表权限失败", ex);
            }
        }
    }
}
