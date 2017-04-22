using ClientLib.Core;
using ClientLib.Entities;
using ClientLib.Utilities;
using Easi365DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IO = System.IO;

namespace ClientLib.SyncActions
{
    public class CreateLocalFile : SyncAction
    {
        const int BUFFER_SIZE = 1024;
        const string CONFIG_FILE_NAME = "Info.xml";

        public override SyncTargetType TargetType
        {
            get { return SyncTargetType.Client; }
        }

        public override SyncActionType SyncType
        {
            get { return SyncActionType.CreateFile; }
        }

        private DB.FileInfoRow _fiRow = null;

        public DB.FileInfoRow FiRow
        {
            get { return _fiRow; }
            set { _fiRow = value; }
        }

        public CreateLocalFile(ServerItem si)
        {
            this.ServerItem = si;
        }

        //public CreateLocalFile(DB.FileInfoRow fiRow, ServerItem si)
        //{
        //    this._fiRow = fiRow;
        //    this.ServerItem = si;
        //}

        public override ServerItem ServerItem { get; set; }
        public override ClientItem ClientItem { get; set; }

        public override int ServerId
        {
            get { return this.ServerItem.Id; }
        }

        public override string ClientId
        {
            get { return null; }
        }

        public override string Path
        {
            get { return this.ServerItem.LibRelativeUrl.Replace('/', '|'); }
        }

        public override string OldPath
        {
            get { return null; }
        }

        /// <summary>
        /// 本地文件路径
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// 创建本地文件夹
        /// </summary>
        /// <param name="folderName">GUID</param>
        private string EnsureLocalFilePath(string folderName)
        {
           // Guid folderName = Guid.NewGuid();
            string localPath = CoreManager.ConfigManager.Settings.PersonalFilesCache +
                IO.Path.DirectorySeparatorChar + folderName.ToString();

            if (!IO.Directory.Exists(localPath))
                IO.Directory.CreateDirectory(localPath);

            this.LocalPath = localPath + IO.Path.DirectorySeparatorChar + ServerItem.Name;

            return localPath;
        }

        public async Task Execute(ServerSide server, DownloadState state)
        {
            if (this.Ignore)
            {
                Logging.AddInfo("忽略操作：" + this.ToString());
                return;
            }
            Logging.AddInfo("开始文件下载（新建）：" + this.Path);

            IO.FileStream fs = null;
            HttpWebRequest req = null;
            HttpWebResponse rsp = null;

            try
            {
                string remotePath = server.MapFullPath(ServerItem.ServerRelativeUrl);

                //使用文件的GUID作为本地文件目录的名称
               string folderPath= EnsureLocalFilePath(this.ServerItem.UniqueId);

                fs = new IO.FileStream(this.LocalPath, IO.FileMode.OpenOrCreate);

                //创建WebRequst
                Uri uri = new Uri(remotePath);
                 req = (HttpWebRequest)WebRequest.Create(uri);
                req.Credentials = server.ClientCtx.Credentials;
                req.KeepAlive = false;
                req.Accept = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
                req.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; MDDC)";
                //下载文件时提示未授权 需要添加这个头信息
                req.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");

               // var rsp = await req.GetResponseAsync() as HttpWebResponse;

               

                try
                {
                    rsp = await req.GetResponseAsync(state.CancellationToken.Token);
                }
                catch (Exception)
                {
                    if (state.CancellationToken.IsCancellationRequested)
                    {
                        DeleteUnfinishedFile(this.LocalPath, fs);
                        state.Reset();
                        return;
                    }
                }

                var stream = rsp.GetResponseStream();

                state.TotalBytes = rsp.ContentLength;
                byte[] buffer = new byte[BUFFER_SIZE];
                var readBytes = await stream.ReadAsync(buffer, 0, BUFFER_SIZE);
                //var readBytes = stream.Read(buffer, 0, BUFFER_SIZE);

                while (readBytes > 0)
                {
                    await fs.WriteAsync(buffer, 0, readBytes);
                    state.BytesRead += readBytes;
                    var percent = ((double)state.BytesRead / (double)state.TotalBytes);
                    state.DownloadPercent = percent;

                    if (state.CancellationToken.IsCancellationRequested)
                    {
                        //取消下载任务后 删除没有下载完成的文件
                        state.Reset();
                        DeleteUnfinishedFile(this.LocalPath, fs);
                        if(rsp !=null)
                            rsp.Close();
                        return;
                    }

                    readBytes = await stream.ReadAsync(buffer, 0, BUFFER_SIZE, state.CancellationToken.Token);
                }

                //fs.Close();
                //fs.Dispose();

                //添加xml文件信息
                var fileInfo = new LocalFileInfo();
                fileInfo.Id = this.ServerItem.UniqueId;
                fileInfo.FileName = this.ServerItem.Name;
                fileInfo.ServerRelativePath = this.ServerItem.ServerRelativeUrl;
                fileInfo.IsNormalFile = true;

                XmlHelper.XmlSerializeToFile(fileInfo,
                    folderPath + "\\" + CONFIG_FILE_NAME, Encoding.UTF8);

                //重置下载计数
                state.Reset();

                ClientItem ci = new Entities.ClientItem(new System.IO.FileInfo(this.LocalPath));


                if (_fiRow == null)
                {
                    server.DB.AddClientItem(ci, this.ServerItem, this.ServerItem.UniqueId);

                    //server.DB.AddSyncLogItem(new SyncLogService.Log()
                    //{
                    //    FileName = ServerItem.Name,
                    //    ComputerName = System.Net.Dns.GetHostName(),
                    //    DepartmentName = "  ",
                    //    IP = "255.255.255.255",
                    //    CreatedTime = DateTime.Now,
                    //    UserAction = SyncLogService.UserAction.Download,
                    //    UserName = server.User.Account,
                    //    LogMessage = "下载 " + ServerItem.Name + " 到本地."
                    //});
                }
                else
                {
                    //更新FileInfoRow条目
                   // _fiRow.CacheFolder = folderPath;
                    _fiRow.CacheFolder = this.ServerItem.UniqueId;
                    server.DB.UpdateClientItem(ci, this.ServerItem, _fiRow);
                }
            }
            catch (System.Net.WebException webEx)
            {
                if (webEx.InnerException == null)
                {
                    // 源文件未找到
                    server.DB.AddErrorInfo("1341", "源文件未找到",
                        "新建到本地 - " + this.Path + Environment.NewLine + webEx.ToString(), this);
                }
                else if (webEx.InnerException is IO.DirectoryNotFoundException)
                {
                    // 父文件夹不存在
                    server.DB.AddErrorInfo("1342", "父文件夹不存在",
                        "新建到本地 - " + this.Path + Environment.NewLine + webEx.ToString(), this);
                }
                else
                {
                    // 其他异常 
                    server.DB.AddErrorInfo("1343", webEx.Message,
                        "新建到本地 - " + this.Path + Environment.NewLine + webEx.ToString(), this);
                }

                throw webEx;
            }
            catch (Exception ex)
            {
                // 其他异常
                server.DB.AddErrorInfo("1343", ex.Message,
                    "新建到本地 - " + this.Path + Environment.NewLine + ex.ToString(), this);

                throw ex;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }

                req = null;
                GC.Collect();
            }
        }

        private void DeleteUnfinishedFile(string path, IO.Stream stream)
        {
            try
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
                IO.FileInfo info = new IO.FileInfo(path);
                if (info.Exists)
                {
                    var dir = info.Directory;
                    info.Delete();
                    dir.Delete(true);
                }
            }
            catch (Exception ex)
            {
                Logging.Add("删除临时文件出错." + path, ex);
            }
        }

        public override void Execute(ServerSide server, bool autoResolveConflict, Action<bool> complete)
        {
            if (this.Ignore)
            {
                Logging.AddInfo("忽略操作：" + this.ToString());
                return;
            }
            Logging.AddInfo("开始文件下载（新建）：" + this.Path);

            try
            {
                string remotePath = server.MapFullPath(ServerItem.ServerRelativeUrl);

                string folderName = Guid.NewGuid().ToString();
                string localPath = CoreManager.ConfigManager.Settings.PersonalFilesCache +
                    IO.Path.DirectorySeparatorChar + folderName;

                if (!IO.Directory.Exists(localPath))
                    IO.Directory.CreateDirectory(localPath);

                this.LocalPath = localPath + IO.Path.DirectorySeparatorChar + ServerItem.Name;

                //server.Downloader.DownloadFile(remotePath, localFile);
                byte[] data = server.Downloader.DownloadData(remotePath);
                IO.File.WriteAllBytes(this.LocalPath, data);

                ClientItem ci = new Entities.ClientItem(new System.IO.FileInfo(this.LocalPath));

                if (_fiRow == null)
                {
                    //将当前文件信息写入到本地数据库
                    //父级文件夹id
                    //if (this.ServerItem != null)
                    //    si.ParentFolderId = this.ServerItem.Id;
                    //else
                    //    si.ParentFolderId = 0;

                    server.DB.AddClientItem(ci, this.ServerItem, folderName);
                }
                else
                {
                    //更新FileInfoRow条目
                    _fiRow.CacheFolder = folderName;
                    server.DB.UpdateClientItem(ci, this.ServerItem, _fiRow);
                }
            }
            catch (System.Net.WebException webEx)
            {
                if (webEx.InnerException == null)
                {
                    // 源文件未找到
                    server.DB.AddErrorInfo("1341", "源文件未找到",
                        "新建到本地 - " + this.Path + Environment.NewLine + webEx.ToString(), this);
                }
                else if (webEx.InnerException is IO.DirectoryNotFoundException)
                {
                    // 父文件夹不存在
                    server.DB.AddErrorInfo("1342", "父文件夹不存在",
                        "新建到本地 - " + this.Path + Environment.NewLine + webEx.ToString(), this);
                }
                else
                {
                    // 其他异常 
                    server.DB.AddErrorInfo("1343", webEx.Message,
                        "新建到本地 - " + this.Path + Environment.NewLine + webEx.ToString(), this);
                }

                throw webEx;
            }
            catch (Exception ex)
            {
                // 其他异常
                server.DB.AddErrorInfo("1343", ex.Message,
                    "新建到本地 - " + this.Path + Environment.NewLine + ex.ToString(), this);

                throw ex;
            }

        }
    }
}
