using ClientLib.Core;
using ClientLib.Entities;
using System;
using System.Collections.Generic;
using IO=System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ClientLib.SyncActions
{
   public  class UpdateRemoteFile : SyncAction
    {
       private string _path;

        public override SyncTargetType TargetType
        {
            get { return SyncTargetType.Server; }
        }

        public override SyncActionType SyncType
        {
            get { return SyncActionType.UpdateFile; }
        }

        public override ServerItem ServerItem { get; set; }
        public override ClientItem ClientItem { get; set; }

        public override int ServerId
        {
            get { throw new NotImplementedException(); }
        }

        public override string ClientId
        {
            get { throw new NotImplementedException(); }
        }
        
        public override string Path
        {
            get { return this._path.Replace('\\', '/'); }
        }

        public override string OldPath
        {
            get { return null; }
        }

       //是否使用绝对地址上传
        public bool UseAbsolutePath { get; set; }

        public UpdateRemoteFile(ClientItem ci, string path)
        {
            this.ClientItem = ci;
            this._path = path;
        }

        public async Task UpdateFile(ServerSide server,string filepath, UploadState uploadState, Action<bool, ServerItem> uploaded)
        {
            bool hasError = false;
            ClientLib.Entities.ServerItem si = null;

            if (this.Ignore)
            {
                server.DB.AddLogMessage("忽略操作:" + this.ToString());
                return;
            }

            server.DB.AddLogMessage("开始文件上传 (更新) :" + this.Path);
            Logging.WriteOperLog("开始文件上传 (更新) :" + this.Path + DateTime.Now);

            // 支持的文件
            //string serverRelPath = server.RootFolderServerRelativeUrl + "/" + _path.Replace('\\', '/').Replace("//", "/").TrimStart('/');
            if (Path.Length >= 260)
            {
                Logging.Add(new Exception(string.Concat("1445", "服务器全路径长度不允许超过260个字符",
                    "更新到服务器 - " + this.Path)));
                return;
            }

            try
            {
                //是否直接使用绝对路径上传文件
                var uri = this.UseAbsolutePath
                    ?new Uri(this.Path) 
                    :new Uri(new Uri(server.ClientCtx.Url), Path);

                IO.FileInfo fileInfo = new IO.FileInfo(filepath);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

                request.ContentType = "multipart/form-data; charset=utf-8";

                request.KeepAlive = false;

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
               
                using (IO.FileStream stream = fileInfo.OpenRead())
                {
                    using (IO.Stream uploadStream = await request.GetRequestStreamAsync())
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead = 0;

                        bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, uploadState.CancellationToken.Token);

                        while (bytesRead > 0)
                        {
                            if (uploadState.CancellationToken.IsCancellationRequested)
                                Debug.WriteLine(string.Format("取消上传任务 before write:{0}.",Path));

                            await uploadStream.WriteAsync(buffer, 0, bytesRead, uploadState.CancellationToken.Token);
                            uploadStream.Flush();

                            if (uploadState.CancellationToken.IsCancellationRequested)
                                Debug.WriteLine(string.Format("取消上传任务:{0}.", Path));

                            uploadState.BytesWrite += bytesRead;
                            uploadState.UploadPercent = ((double)uploadState.BytesWrite / (double)uploadState.TotalBytes);

                            bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, uploadState.CancellationToken.Token);
                        }
                    }
                }

                var rsp = await request.GetResponseAsync() as HttpWebResponse;

                if (rsp != null && (rsp.StatusCode == HttpStatusCode.Created || rsp.StatusCode == HttpStatusCode.OK))
                {
                    si = await server.GetServerItemByListItemAsync(Path, 0);
               
                    if (si != null)
                    {
                        //server.DB.AddUploadedItem(si);
                       // server.DB.UpdateClientItem(ClientItem, si, server.DB.LocalDict.FilePathHashDict[ClientItem.PathHash]);
                        server.DB.UpdateClientItem(
                            ClientItem, 
                            si, 
                            server.DB.LocalDict.ServerUniqueId[si.UniqueId]);
                    }
             
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

        public override void Execute(ServerSide server, bool autoResolveConflict, Action<bool> complete)
        {
            throw new NotImplementedException();
        }
    }
}
