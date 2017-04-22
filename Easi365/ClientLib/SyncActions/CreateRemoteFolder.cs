using ClientLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SP = Microsoft.SharePoint.Client;
using ClientLib.Core;
using ClientLib.Entities;
using System.Threading.Tasks;

namespace ClientLib.SyncActions
{
    public class CreateRemoteFolder : SyncAction
    {
        public override SyncTargetType TargetType
        {
            get { return SyncTargetType.Server; }
        }

        public override SyncActionType SyncType
        {
            get { return SyncActionType.CreateFolder; }
        }

        public override ServerItem ServerItem { get; set; }
        public override ClientItem ClientItem { get; set; }

        public override int ServerId
        {
            get { return 0; }
        }

        public override string ClientId
        {
            get { return ClientItem.FileIndex; }
        }

        public override string Path
        {
            get { return _path.Replace("\\", "|"); }
        }

        public override string OldPath
        {
            get { return null; }
        }

        private string _path;
        public CreateRemoteFolder(ClientItem ci, string path)
        {
            ClientItem = ci;
            _path = path;
        }

        public Task<ClientLib.Entities.ServerItem> Execute(ServerSide server, bool autoResolveConflict)
        {
            return Task.Factory.StartNew<ClientLib.Entities.ServerItem>(() => 
            {
                ClientLib.Entities.ServerItem si = null;

                if (this.Ignore)
                {
                    server.DB.AddLogMessage("忽略操作:" + this.ToString());
                    return si;
                }

                try
                {
                    string serverRelPath = server.RootFolderServerRelativeUrl + "/" + _path.Replace('\\', '/');
                    if (serverRelPath.Length > 260)
                    {
                        Logging.Add(string.Concat("1413", "服务器全路径长度不允许超过260个字符",
                           "创建远程文件夹 - " + this.Path), null);
                    }

                    Logging.WriteOperLog("开始创建文件夹 (新建):" + _path.Substring(_path.LastIndexOf('\\') + 1) + DateTime.Now);

                    // 创建文件夹
                    SP.ListItem folderItm = null;
                 
                    SP.List library = server.RemoteLibrary;
                    var createInfo = new SP.ListItemCreationInformation();
                    createInfo.UnderlyingObjectType = SP.FileSystemObjectType.Folder;
                    if (_path.IndexOf('\\') > -1)
                    {
                        createInfo.FolderUrl = server.RootFolderServerRelativeUrl + "/" +
                            _path.Substring(0, _path.LastIndexOf('\\')).Replace('\\', '/');
                    }
                    createInfo.LeafName = _path.Substring(_path.LastIndexOf('\\') + 1);
                    folderItm = library.AddItem(createInfo);
                    folderItm.Update();
                    server.ClientCtx.Load(folderItm,
                        itm => itm.Id,
                        itm => itm.FileSystemObjectType,
                        itm => itm["FileLeafRef"],
                        itm => itm["FileRef"],
                        itm => itm["owshiddenversion"],
                        itm => itm["Modified"],
                        itm => itm["UniqueId"]
                        );
                    server.ClientCtx.ExecuteQuery();
                    si = new ServerItem(folderItm);

                    //server.DB.AddSyncLogItem(new SyncLogService.Log()
                    //{
                    //    FileName = si.Name,
                    //    ComputerName = System.Net.Dns.GetHostName(),
                    //    DepartmentName = "  ",
                    //    IP = "255.255.255.255",
                    //    CreatedTime = DateTime.Now,
                    //    UserAction = SyncLogService.UserAction.Delete,
                    //    UserName = server.User.Account,
                    //    LogMessage = "创建文件夹 " + si.Name
                    //});

                    return si;
                }
                catch (Exception ex)
                {
                    //出现其他异常
                    Logging.Add(string.Concat("1412", ex.Message,
                        "创建远程文件夹 - " + this.Path), ex);
                }

                return si;
            });
        }

        public override void Execute(ServerSide server, bool autoResolveConflict, Action<bool> done)
        {
            ClientLib.Entities.ServerItem si = null;

            if (this.Ignore)
            {
                server.DB.AddLogMessage("忽略操作:" + this.ToString());
                return;
            }

            try
            {
                string serverRelPath = server.RootFolderServerRelativeUrl + "/" + _path.Replace('\\', '/');
                if (serverRelPath.Length >260)
                {
                    Logging.Add(string.Concat("1413", "服务器全路径长度不允许超过260个字符",
                       "创建远程文件夹 - " + this.Path), null);

                    //server.DB.AddErrorInfo("1413", "服务器全路径长度不允许超过260个字符",
                    //   "创建远程文件夹 - " + this.Path, this);
                    // 处理依赖
                }

                Logging.WriteOperLog("开始创建文件夹 (新建):" + _path.Substring(_path.LastIndexOf('\\') + 1) + DateTime.Now);

                // 创建文件夹
                SP.ListItem folderItm = null;
                //SP.ExceptionHandlingScope exScope = new SP.ExceptionHandlingScope(server.ClientCtx);
                //using (exScope.StartScope())
                //{
                //    using (exScope.StartTry())
                //    {
                        
                //    }
                //    using (exScope.StartCatch())
                //    {
                //    }
                //}
                SP.List library = server.RemoteLibrary;
                var createInfo = new SP.ListItemCreationInformation();
                createInfo.UnderlyingObjectType = SP.FileSystemObjectType.Folder;
                if (_path.IndexOf('\\') > -1)
                {
                    createInfo.FolderUrl = server.RootFolderServerRelativeUrl + "/" +
                        _path.Substring(0, _path.LastIndexOf('\\')).Replace('\\', '/');
                }
                createInfo.LeafName = _path.Substring(_path.LastIndexOf('\\') + 1);
                folderItm = library.AddItem(createInfo);
                folderItm.Update();
                server.ClientCtx.Load(folderItm,
                    itm => itm.Id,
                    itm => itm.FileSystemObjectType,
                    itm => itm["FileLeafRef"],
                    itm => itm["FileRef"],
                    itm => itm["owshiddenversion"],
                    itm => itm["Modified"]);
                server.ClientCtx.ExecuteQuery();
                if(done!=null)
                    done(true);

                //if (exScope.HasException)
                //{
                //    Logging.AddInfo("创建远程文件夹失败." + exScope.ErrorMessage);
                //    done(false);
                //}
                //else
                //{
                //    //string createdPath = CoreManager.ConfigManager.Settings.PersonalFilesFolder + System.IO.Path.DirectorySeparatorChar + _path;

                //    //if (!System.IO.Directory.Exists(createdPath))
                //    //    System.IO.Directory.CreateDirectory(createdPath);

                //    //ClientItem ci = new Entities.ClientItem(new System.IO.DirectoryInfo(createdPath));

                //    //ServerItem si = new ServerItem(folderItm);
                //    ////父级文件夹id
                //    //if (this.ServerItem != null)
                //    //    si.ParentFolderId = this.ServerItem.Id;
                //    //else
                //    //    si.ParentFolderId = 0;

                //    //server.DB.AddClientItem(ci, si);
                //    done(true);
                //}
            }
            catch (Exception ex)
            {
                //出现其他异常
                Logging.Add(string.Concat("1412", ex.Message,
                    "创建远程文件夹 - " + this.Path), ex);
                if (done != null)
                    done(false);
            }
        }

        private SP.Folder EnsureFolder(ServerSide server, SP.Folder parentFolder, string folderPath)
        {
            string[] pathElements = folderPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string head = pathElements[0];
            var newFolder = parentFolder.Folders.Add(head);
            server.ClientCtx.Load(newFolder);
            server.ClientCtx.ExecuteQuery();

            if (pathElements.Length > 1)
            {
                string tail = string.Empty;
                for (int i = 1; i < pathElements.Length; i++)
                    tail = tail + "/" + pathElements[i];

                return EnsureFolder(server, newFolder, tail);
            }
            else
            {
                return newFolder;
            }
        }
    }
}
