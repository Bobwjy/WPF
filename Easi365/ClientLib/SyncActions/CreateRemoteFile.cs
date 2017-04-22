using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientLib.Entities;
using SP = Microsoft.SharePoint.Client;
using IO = System.IO;
using ClientLib.Core;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace ClientLib.SyncActions
{
    public class CreateRemoteFile : SyncAction
    {
        public override SyncTargetType TargetType
        {
            get { return SyncTargetType.Server; }
        }

        public override SyncActionType SyncType
        {
            get { return SyncActionType.CreateFile; }
        }

        public override ServerItem ServerItem { get; set; }
        public override ClientItem ClientItem { get; set; }

        private int _serverId;
        public override int ServerId
        {
            get { return this._serverId; }
        }

        public override string ClientId
        {
            get { return this.ClientItem.FileIndex; }
        }

        private string _path;
        public override string Path
        {
            get { return this._path.Replace('\\', '/'); }
        }

        public override string OldPath
        {
            get { return null; }
        }

        public CreateRemoteFile(ClientItem ci, string path)
        {
            this.ClientItem = ci;
            this._path = path;
        }

        public async Task Execute(ServerSide server, UploadState uploadState, Action<bool, ServerItem> uploaded)
        {
            bool hasError = false;
            ClientLib.Entities.ServerItem si = null;

            if (this.Ignore)
            {
                server.DB.AddLogMessage("忽略操作:" + this.ToString());
                return;
            }

            server.DB.AddLogMessage("开始文件上传 (新建) :" + this.Path);
            Logging.WriteOperLog("开始文件上传 (新建) :" + this.Path + DateTime.Now);

            // 支持的文件
            string serverRelPath = server.RootFolderServerRelativeUrl + "/" + _path.Replace('\\', '/').Replace("//", "/").TrimStart('/');
            if (serverRelPath.Length >= 260)
            {
                Logging.Add(new Exception(string.Concat("1444", "服务器全路径长度不允许超过260个字符",
                    "新建到服务器 - " + this.Path)));
                return;
            }

            try
            {
                var uri = new Uri(new Uri(server.ClientCtx.Url), serverRelPath);
                FileInfo fileInfo = new FileInfo(ClientItem.BasicInfo.FullName);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

                request.ContentType = "multipart/form-data; charset=utf-8";

                request.Credentials = server.ClientCtx.Credentials;
                request.AllowWriteStreamBuffering = true;
                request.Method = "PUT";
                request.ContentLength = fileInfo.Length;
                //request.Accept = "*/*";
                request.Accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                //request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; MDDC)";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                //下载文件时提示未授权 需要添加这个头信息
                request.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                request.Headers.Add("Translate", "F");
                request.Headers.Add("Cache-Control", "no-cache");

                uploadState.TotalBytes = fileInfo.Length;

                using (FileStream stream = fileInfo.OpenRead())
                {
                    using (Stream uploadStream = await request.GetRequestStreamAsync())
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead = 0;
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, uploadState.CancellationToken.Token)) > 0)
                        {
                            await uploadStream.WriteAsync(buffer, 0, bytesRead, uploadState.CancellationToken.Token);
                            uploadStream.Flush();
                            uploadState.BytesWrite += bytesRead;
                            uploadState.UploadPercent = ((double)uploadState.BytesWrite / (double)uploadState.TotalBytes);
                        }
                    }
                }

                var rsp = await request.GetResponseAsync() as HttpWebResponse;

                if (rsp != null && (rsp.StatusCode == HttpStatusCode.Created || rsp.StatusCode == HttpStatusCode.OK))
                {
                    //PersonalServerSide personalServer = server as PersonalServerSide;
                    //if (personalServer != null)
                    //{
                    si = await server.GetServerItemByListItemAsync(serverRelPath, 0);
                    //将已经上传的文件信息添加到本地数据库中
                    if (si != null)
                    {
                        server.DB.AddUploadedItem(si);

                        try
                        {
                            server.DB.AddSyncLogItem(new SyncLogService.Log()
                            {
                                FileName = fileInfo.Name,
                                ComputerName = System.Net.Dns.GetHostName(),
                                DepartmentName = "  ",
                                IP = "255.255.255.255",
                                CreatedTime = DateTime.Now,
                                UserAction = SyncLogService.UserAction.Upload,
                                UserName = server.User.Account,
                                LogMessage = "上传 " + fileInfo.Name + " 到服务器."
                            });
                        }
                        catch { }
                    }
                    //}
                }
            }
            catch (IO.FileNotFoundException fnfEx)
            {
                hasError = true;
                Logging.Add(string.Concat("1441", "源文件未找到",
                    "新建到服务器 - " + this.Path), fnfEx);
                server.DB.AddErrorInfo("1441", "源文件未找到",
                    "新建到服务器 - " + this.Path + Environment.NewLine + fnfEx.ToString(), this);
            }
            catch (IO.DirectoryNotFoundException dnfEx)
            {
                hasError = true;
                Logging.Add(string.Concat("1441", "源文件未找到",
                   "新建到服务器 - " + this.Path), dnfEx);

                server.DB.AddErrorInfo("1441", "源文件未找到",
                    "新建到服务器 - " + this.Path + Environment.NewLine + dnfEx.ToString(), this);
            }
            catch (Exceptions.ServerFullException sf)
            {
                hasError = true;
                Logging.Add(string.Concat("1445", "服务器配额空间已满，请联系管理员",
                    "新建到服务器 - " + this.Path), sf);

                server.DB.AddErrorInfo("1445", "服务器配额空间已满，请联系管理员",
                    "新建到服务器 - " + this.Path, this);
            }
            catch (Exceptions.UploadException upEx)
            {
                hasError = true;
                Logging.Add(string.Concat("1443", upEx.Message,
                    "新建到服务器 - " + this.Path), upEx);

                server.DB.AddErrorInfo("1443", upEx.Message,
                    "新建到服务器 - " + this.Path + Environment.NewLine + upEx.ToString(), this);
            }
            catch (Exception ex)
            {
                hasError = true;
                Logging.Add(string.Concat("1443", ex.Message,
                    "新建到服务器 - " + this.Path), ex);
                //"出现其他异常 - " 
                server.DB.AddErrorInfo("1443", ex.Message,
                    "新建到服务器 - " + this.Path + Environment.NewLine + ex.ToString(), this);
            }

            if (uploaded != null)
                uploaded(hasError, si);
        }

        public override void Execute(ServerSide server, bool autoResolveConflict, Action<bool> uploaded)
        {
            bool hasError = false;

            if (this.Ignore)
            {
                server.DB.AddLogMessage("忽略操作:" + this.ToString());
                return;
            }

            server.DB.AddLogMessage("开始文件上传 (新建) :" + this.Path);
            Logging.WriteOperLog("开始文件上传 (新建) :" + this.Path + DateTime.Now);

            // 支持的文件
            string serverRelPath = server.RootFolderServerRelativeUrl + "/" + _path.Replace('\\', '/');
            if (serverRelPath.Length >= 260)
            {
                Logging.Add(new Exception(string.Concat("1444", "服务器全路径长度不允许超过260个字符",
                    "新建到服务器 - " + this.Path)));

                //server.DB.AddErrorInfo("1444", "服务器全路径长度不允许超过260个字符",
                //    "新建到服务器 - " + this.Path, this);
                return;
            }

            try
            {
                //上传文件
                //using (IO.FileStream stream = new IO.FileStream(ClientItem.BasicInfo.FullName,
                //    IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.ReadWrite))
                //{
                //    Utilities.ClientContextExtension.ClearPendingExecution(server.ClientCtx);
                //    Utilities.ClientContextExtension.SaveFileDirectWithoutOverwrite(server.ClientCtx,
                //        serverRelPath, stream, uploaded);
                //}
                //var uploadedFile = server.ClientCtx.Web.GetFileByServerRelativeUrl(serverRelPath);
                //server.ClientCtx.Load(uploadedFile.ListItemAllFields,
                //    itm => itm.Id,
                //    itm => itm.FileSystemObjectType,
                //    itm => itm["FileLeafRef"],
                //    itm => itm["FileRef"],
                //    itm => itm["owshiddenversion"],
                //    itm => itm["Modified"]);
                //server.ClientCtx.ExecuteQuery();


                string absolutePath = (new Uri(new Uri(server.ClientCtx.Url), serverRelPath)).AbsoluteUri;

                SP.FileCreationInformation newFile = new SP.FileCreationInformation();
                newFile.ContentStream = new IO.FileStream(ClientItem.BasicInfo.FullName, IO.FileMode.Open);
                //newFile.Url = server.DB.LocalDict.FilePathHashDict[this.PathHash].ServerRelativeUrl;
                newFile.Url = absolutePath;
                newFile.Overwrite = true;

                SP.File uploadFile = server.RemoteLibrary.RootFolder.Files.Add(newFile);
                uploadFile.ListItemAllFields.Update();

                server.ClientCtx.Load(uploadFile.ListItemAllFields,
                    itm => itm.Id,
                    itm => itm.FileSystemObjectType,
                    itm => itm["FileLeafRef"],
                    itm => itm["FileRef"],
                    itm => itm["owshiddenversion"],
                    itm => itm["Modified"]);
                server.ClientCtx.ExecuteQuery();

                //ServerItem si = new ServerItem(uploadedFile.ListItemAllFields);
                //_serverId = si.Id;

                //if (this.ServerItem != null)
                //    si.ParentFolderId = this.ServerItem.Id;

                ////写入数据库
                //server.DB.AddClientItem(ClientItem, si);
            }
            catch (IO.FileNotFoundException fnfEx)
            {
                hasError = true;
                Logging.Add(string.Concat("1441", "源文件未找到",
                    "新建到服务器 - " + this.Path), fnfEx);
                server.DB.AddErrorInfo("1441", "源文件未找到",
                    "新建到服务器 - " + this.Path + Environment.NewLine + fnfEx.ToString(), this);
            }
            catch (IO.DirectoryNotFoundException dnfEx)
            {
                hasError = true;
                Logging.Add(string.Concat("1441", "源文件未找到",
                   "新建到服务器 - " + this.Path), dnfEx);

                server.DB.AddErrorInfo("1441", "源文件未找到",
                    "新建到服务器 - " + this.Path + Environment.NewLine + dnfEx.ToString(), this);
            }
            catch (Exceptions.ServerFullException sf)
            {
                hasError = true;
                Logging.Add(string.Concat("1445", "服务器配额空间已满，请联系管理员",
                    "新建到服务器 - " + this.Path), sf);

                server.DB.AddErrorInfo("1445", "服务器配额空间已满，请联系管理员",
                    "新建到服务器 - " + this.Path, this);
            }
            catch (Exceptions.UploadException upEx)
            {
                hasError = true;
                Logging.Add(string.Concat("1443", upEx.Message,
                    "新建到服务器 - " + this.Path), upEx);

                server.DB.AddErrorInfo("1443", upEx.Message,
                    "新建到服务器 - " + this.Path + Environment.NewLine + upEx.ToString(), this);
            }
            catch (Exception ex)
            {
                hasError = true;
                Logging.Add(string.Concat("1443", ex.Message,
                    "新建到服务器 - " + this.Path), ex);
                //"出现其他异常 - " 
                server.DB.AddErrorInfo("1443", ex.Message,
                    "新建到服务器 - " + this.Path + Environment.NewLine + ex.ToString(), this);
            }
            if (uploaded != null)
                uploaded(hasError);
        }
    }
}
