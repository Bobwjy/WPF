using ClientLib.Entities;
using Easi365DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientLib.Core;
using SP = Microsoft.SharePoint.Client;

namespace ClientLib.SyncActions
{
    public class DeleteRemoteFolder : SyncAction
    {
        public override SyncTargetType TargetType
        {
            get { return SyncTargetType.Server; }
        }

        public override SyncActionType SyncType
        {
            get { return SyncActionType.DeleteFolder; }
        }

        public override ServerItem ServerItem { get; set; }
        public override ClientItem ClientItem { get; set; }



        public override int ServerId
        {
            get { return _fiRow.ServerIndex; }
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

        private DB.FileInfoRow _fiRow = null;
        private string _path;

        public DeleteRemoteFolder(DB.FileInfoRow fiRow, string path)
        {
            _fiRow = fiRow;
            _path = path;
        }

        public DeleteRemoteFolder(ServerItem si, string path)
        {
            this.ServerItem = si;
            _path = path;
        }

        public override void Execute(ServerSide server, bool autoResolveConflict, Action<bool> complete)
        {
            if (this.Ignore)
            {
                Logging.AddInfo("忽略操作:" + this.ToString());
                return;
            }

            Logging.AddInfo("开始删除远程文件夹:" + _path);
            Logging.WriteOperLog("开始删除远程文件夹:" + _path + DateTime.Now);

            try
            {
                //检查当前文件字典中是否有当前文件的信息
                if (server.DB.LocalDict.ServerIdDict.ContainsKey(this.ServerItem.Id))
                    _fiRow = server.DB.LocalDict.ServerIdDict[this.ServerItem.Id];

                if (_fiRow != null) 
                {
                    if (_fiRow.RowState == System.Data.DataRowState.Deleted || _fiRow.RowState == System.Data.DataRowState.Detached)
                    {
                        Logging.AddInfo("试图删除远程文件夹时，文件夹已删除。");
                        return;
                    }
                }

                //if (this.ServerItem.Id == 0)
                //{
                //    // 不被服务器端支持的文件，并没有上传，仅从本地删除
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
                    // 删除失败
                    if (exScope.ServerErrorCode == -2147024809)
                    {
                        // 服务器端已删除
                        Logging.AddInfo("试图删除远程文件夹时，文件夹已删除。");
                       
                    }
                    else if (exScope.ServerErrorCode == -2130575306)
                    {
                        // 文件被锁定
                        Logging.AddInfo(string.Concat("1431", "目标文件夹中部分文件正在被其他应用锁定",
                            "删除远程文件夹 - " + this.Path + Environment.NewLine + exScope.ErrorMessage));
                    }
                    else
                    {
                        // 出现其他异常
                        Logging.AddInfo(string.Concat("1432", exScope.ErrorMessage,
                            "删除远程文件夹 - " + _path + Environment.NewLine + exScope.ErrorMessage));
                    }
                }
                else
                {
                    //本地目录地址
                    string localPath = CoreManager.ConfigManager.Settings.PersonalFilesFolder 
                        + System.IO.Path.DirectorySeparatorChar 
                        + _path;
                    //删除本地数据库中的信息
                    if(_fiRow != null)
                        server.DB.DeleteClientItemFolder(_fiRow);

                    server.DB.AddSyncLogItem(new SyncLogService.Log()
                    {
                        FileName = ServerItem.Name,
                        ComputerName = System.Net.Dns.GetHostName(),
                        DepartmentName = "  ",
                        IP = "255.255.255.255",
                        CreatedTime = DateTime.Now,
                        UserAction = SyncLogService.UserAction.Delete,
                        UserName = server.User.Account,
                        LogMessage = "删除文件夹 " + ServerItem.Name
                    });
                }
            }
            catch (Exception ex)
            {
                Logging.Add("删除远程文件夹失败.", ex);
            }
        }
    }
}
