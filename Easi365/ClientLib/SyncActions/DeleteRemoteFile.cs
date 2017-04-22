using ClientLib.Core;
using ClientLib.Entities;
using Easi365DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SP = Microsoft.SharePoint.Client;

namespace ClientLib.SyncActions
{
    public class DeleteRemoteFile : SyncAction
    {
        public override SyncTargetType TargetType
        {
            get { return SyncTargetType.Server; }
        }

        public override SyncActionType SyncType
        {
            get { return SyncActionType.DeleteFile; }
        }

        public override ServerItem ServerItem { get; set; }
        public override ClientItem ClientItem { get; set; }

        public override int ServerId
        {
            get { return _fiRow.IsServerIndexNull() ? 0 : _fiRow.ServerIndex; }
        }

        public override string ClientId
        {
            get { return _fiRow.FileIndex; }
        }

        public override string Path
        {
            get { return _path.Replace('\\', '|'); }
        }

        public override string OldPath
        {
            get { return null; }
        }

        private DB.FileInfoRow _fiRow;
        private string _path;
        public DeleteRemoteFile(DB.FileInfoRow fiRow, string path)
        {
            _fiRow = fiRow;
            _path = path;
        }

        public DeleteRemoteFile(ServerItem si, string path)
        {
            this.ServerItem = si;
            this._path = path;
        }

        public override void Execute(ServerSide server, bool autoResolveConflict, Action<bool> complete)
        {
            if (this.Ignore)
            {
                Logging.AddInfo("忽略操作：" + this.ToString());
                return;
            }

            Logging.AddInfo("开始删除远程文件：" + this.Path);
            Logging.WriteOperLog("开始删除远程文件：" + this.Path + DateTime.Now);

            try
            {
                //if (_fiRow != null)
                //{
                //    if (_fiRow.RowState == System.Data.DataRowState.Deleted || _fiRow.RowState == System.Data.DataRowState.Detached)
                //    {
                //        Logging.AddInfo("试图删除远程文件时，文件已删除。");
                //        return;
                //    }
                //}

                //if (this.ServerItem.Id == 0)
                //{
                //    // 不被服务器端支持的文件，并没有上传，仅从本地删除
                //    server.DB.DeleteClientItem(_fiRow);
                //    return;
                //}

                // 从服务器端删除
                SP.ExceptionHandlingScope exScope = new SP.ExceptionHandlingScope(server.ClientCtx);
                using (exScope.StartScope())
                {
                    using (exScope.StartTry())
                    {
                        //var item = server.RemoteLibrary.GetItemById(this.ServerId);
                        var item = server.RemoteLibrary.GetItemById(this.ServerItem.Id);
                        item.Recycle();
                    }
                    using (exScope.StartCatch())
                    {
                    }
                }
                server.ClientCtx.ExecuteQuery();

                if (exScope.HasException)
                {
                    Logging.WriteOperLog("删除文件夹失败.");
                    // 删除失败
                    if (exScope.ServerErrorCode == -2147024809)
                    {
                        // 服务器端已删除
                        server.DB.AddLogMessage("试图删除远程文件时，远程文件已删除。");
                        server.DB.DeleteClientItem(_fiRow);
                    }
                    else if (exScope.ServerErrorCode == -2130575306)
                    {
                        // 文件被锁定
                        server.DB.AddErrorInfo("1471", "目标文件正在被其他应用锁定",
                            "删除远程文件 - " + this.Path + Environment.NewLine + exScope.ErrorMessage, this);
                    }
                    else
                    {
                        //出现其他异常
                        server.DB.AddErrorInfo("1472", exScope.ErrorMessage,
                            "删除远程文件 - " + this.Path + Environment.NewLine + exScope.ErrorMessage, this);
                    }
                }
                else
                {
                    //删除本地数据库中条目信息
                    if (server.DB.LocalDict.ServerIdDict.ContainsKey(this.ServerItem.Id))
                        _fiRow = server.DB.LocalDict.ServerIdDict[this.ServerItem.Id];
                    if (_fiRow != null)
                        server.DB.DeleteClientItem(_fiRow);

                    server.DB.AddSyncLogItem(new SyncLogService.Log()
                    {
                        FileName = ServerItem.Name,
                        ComputerName = System.Net.Dns.GetHostName(),
                        DepartmentName = "  ",
                        IP = "255.255.255.255",
                        CreatedTime = DateTime.Now,
                        UserAction = SyncLogService.UserAction.Delete,
                        UserName = server.User.Account,
                        LogMessage = "删除文件 " + ServerItem.Name
                    });
                }
            }
            catch (Exception ex)
            {
                //出现其他异常
                server.DB.AddErrorInfo("1472", ex.Message,
                    "删除远程文件 - " + this.Path + Environment.NewLine + ex.ToString(), this);
            }
        }
    }
}
