using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientLib.Entities;
using Easi365DB.DBTableAdapters;
using Easi365DB;
using ClientLib.Exceptions;
using System.IO;
using ClientLib.SyncActions;
using ClientLib.Core;
using System.Data.OleDb;
using System.Threading.Tasks;
using SyncLogSer = ClientLib.SyncLogService;

namespace ClientLib
{
    public class LocalDB
    {
        private string _connectionString;
        public LocalFilesDict LocalDict { get; set; }
        public TableAdapterManager Adapter { get; private set; }
        public DB SyncDB { get; private set; }

        /// <summary>
        /// 同步日志token
        /// </summary>
        public int? SyncLogToken
        {
            get
            {
                try
                {
                    DB.SyncLogInfoRow infoRow = Adapter.SyncLogInfoTableAdapter.GetData().Rows[0] as DB.SyncLogInfoRow;
                    if (infoRow.IsSyncValueNull())
                        return null;
                    else
                        return infoRow.SyncValue;
                }
                catch (Exception ex)
                {
                    throw new DBException("从数据库中读取SyncLogToken失败.", ex);
                }
            }

            set
            {
                try
                {
                    if (value.HasValue)
                    {
                        DB.SyncLogInfoRow infoRow = Adapter.SyncLogInfoTableAdapter.GetData().Rows[0] as DB.SyncLogInfoRow;
                        infoRow.SyncValue = value.Value;
                        Adapter.SyncLogInfoTableAdapter.Update(infoRow);
                    }
                }
                catch (Exception ex)
                {
                    throw new DBException("向数据库中设置SyncLogToken失败.", ex);
                }

            }
        }


        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="databasePath">数据库文件路径</param>
        public LocalDB(string databasePath)
        {
            _connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + databasePath;
            Adapter = new TableAdapterManager();
            Adapter.InitAdapter(_connectionString);
            SyncDB = new DB();
        }
        #endregion

        public void AddClientItem(ClientItem ci, ServerItem si, string cacheFolder = "")
        {
            DB.FileInfoRow row = SyncDB.FileInfo.NewFileInfoRow();
            row.ADSIndex = ci.ADSId;
            row.Extension = ci.BasicInfo.Extension;
            row.FileIndex = ci.FileIndex;
            row.Hash = ci.Hash;
            row.ItemName = ci.BasicInfo.Name;
            row.ItemSize = (int)ci.Size;
            row.ItemType = (byte)(((ci.BasicInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory) ? 1 : 0);
            row.LastWrite = ci.BasicInfo.LastWriteTime;

            row.Uid = si == null ? "" : si.UniqueId;
            row.ServerIndex = si == null ? 0 : si.Id;
            row.ServerVersion = si == null ? 0 : si.Version;
            if (ci.IsParentFolderSet)
                row.ParentFolder = ci.ParentFolderIndex;
            else if (si != null && si.ParentFolderId > 0 && LocalDict.ServerIdDict.ContainsKey(si.ParentFolderId))
                row.ParentFolder = LocalDict.ServerIdDict[si.ParentFolderId].FileIndex;

            row.PathHash = ci.PathHash;
            row.ServerRelativeUrl = si.ServerRelativeUrl;

            if (!string.IsNullOrWhiteSpace(cacheFolder))
                row.CacheFolder = cacheFolder;

            LocalDict.FileIndexDict[ci.FileIndex] = row;
            if (si != null)
            {
                LocalDict.ServerIdDict[si.Id] = row;
                LocalDict.ServerUniqueId[si.UniqueId] = row;
            }

            // LocalDict.FilePathHashDict[ci.PathHash] = row;

            SyncDB.FileInfo.Rows.Add(row);
            Adapter.FileInfoTableAdapter.Update(row);
            SyncDB.FileInfo.AcceptChanges();
        }

        public void AddUploadedItem(ServerItem si)
        {
            DB.UploadingFileDetailRow row = SyncDB.UploadingFileDetail.NewUploadingFileDetailRow();
            row.FileName = si.Name;
            row.Size = si.FileSize;
            row.CreatedDate = si.Modified;

            row.Success = true;

            SyncDB.UploadingFileDetail.Rows.Add(row);
            Adapter.UploadingFileDetailTableAdapter.Update(row);
            SyncDB.UploadingFileDetail.AcceptChanges();
        }

        /// <summary>
        /// 同步到服务器端的日志信息
        /// </summary>
        /// <param name="log"></param>
        public void AddSyncLogItem(SyncLogSer.Log log)
        {
            DB.SyncLogRow row = SyncDB.SyncLog.NewSyncLogRow();
            row.ComputerName = log.ComputerName;
            row.DepartmentName = log.DepartmentName;
            row.FileName = log.FileName;
            row.UserAction = log.UserAction.ToString();
            row.UserName = log.UserName;
            row.CreatedTime = log.CreatedTime.ToLocalTime();
            row.LogMsg = log.LogMessage;

            SyncDB.SyncLog.Rows.Add(row);
            Adapter.SyncLogTableAdapter.Update(row);
            SyncDB.SyncLog.AcceptChanges();
        }

        /// <summary>
        /// 获取同步日志信息 每次同步20条
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<SyncLogSer.Log> GetSyncLogsByID(int? id)
        {
            try
            {
                using (OleDbConnection connection = new OleDbConnection(Adapter.Connection.ConnectionString))
                {
                    connection.Open();
                    List<SyncLogSer.Log> logs = new List<SyncLogSer.Log>();

                    StringBuilder sqlBuilder = new StringBuilder();
                    sqlBuilder.Append("SELECT TOP 20 SyncLog.ID,SyncLog.ComputerName, SyncLog.UserAction, SyncLog.FileName, SyncLog.CreatedTime, SyncLog.DepartmentName, SyncLog.UserName, SyncLog.LogMsg");
                    sqlBuilder.Append(" FROM SyncLog");
                    //if(date.HasValue)
                    //    sqlBuilder.Append(" WHERE (((SyncLog.CreatedTime)>#"+date.Value+"#))");
                    if (id.HasValue)
                        sqlBuilder.Append(" WHERE (((SyncLog.ID)>" + id.Value + "))");
                    sqlBuilder.Append(" ORDER BY SyncLog.CreatedTime, SyncLog.ID");

                    OleDbCommand command = new OleDbCommand(sqlBuilder.ToString(), connection);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        SyncLogSer.Log log = new SyncLogSer.Log();
                        log.ID = Convert.ToInt32(reader["ID"]);
                        log.ComputerName = Convert.ToString(reader["ComputerName"]);
                        log.DepartmentName = Convert.ToString(reader["DepartmentName"]);
                        log.FileName = Convert.ToString(reader["FileName"]);
                        log.UserAction = (SyncLogSer.UserAction)Enum.Parse(typeof(SyncLogSer.UserAction), Convert.ToString(reader["UserAction"]));
                        log.CreatedTime = Convert.ToDateTime(reader["CreatedTime"]);
                        log.UserName = Convert.ToString(reader["UserName"]);
                        log.LogMessage = Convert.ToString(reader["LogMsg"]);
                        logs.Add(log);
                    }

                    return logs;
                }
            }
            catch (Exception ex)
            {
                throw new DBException("获取SyncLog数据失败.", ex);
            }
        }

        public async Task ClearUploadedHistoryAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    using (OleDbConnection connection = new OleDbConnection(Adapter.Connection.ConnectionString))
                    {
                        connection.Open();
                        int count = 0;
                        //清理上传文件历史记录
                        OleDbCommand command = new OleDbCommand("DELETE FROM UploadingFileDetail", connection);
                        count = command.ExecuteNonQuery();
                        Logging.AddInfo(string.Format("清除 {0} 条文件上传记录。", count));
                    }
                }
                catch (Exception ex)
                {
                    Logging.Add("删除文件上传历史记录错误 ", ex);
                }
            });
        }

        #region 记录日志和异常

        public void AddLogMessage(string message)
        {
            try
            {
                Adapter.LogTableAdapter.Insert(DateTime.Now, message);
            }
            catch (Exception ex)
            {
                throw new DBException("数据库日志记录失败.", ex);
            }
        }

        /// <summary>
        /// 记录异常信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="detail"></param>
        public void AddErrorInfo(string code, string message, string detail, SyncAction action)
        {
            try
            {
                if (action != null)
                    Adapter.ErrorsTableAdapter.Insert(code, DateTime.Now,
                        System.IO.Path.GetFileName(action.Path.Replace('|', '\\')),
                            action.Path, action.OldPath, message, detail, false, false);
                else
                    Adapter.ErrorsTableAdapter.Insert(code, DateTime.Now, null, null, null, message, detail, false, false);
            }
            catch (Exception ex)
            {
                throw new DBException("数据库错误信息记录失败.", ex);
            }
        }
        #endregion

        /// <summary>
        /// 删除数据库中本地文件信息
        /// </summary>
        /// <param name="fileInfoRow"></param>
        public void DeleteClientItem(DB.FileInfoRow fileInfoRow)
        {
            var fileInfoDataTable = Adapter.FileInfoTableAdapter.GetData();

            if (LocalDict.FileIndexDict.ContainsKey(fileInfoRow.FileIndex))
                LocalDict.FileIndexDict.Remove(fileInfoRow.FileIndex);
            if (!fileInfoRow.IsServerIndexNull() && LocalDict.ServerIdDict.ContainsKey(fileInfoRow.ServerIndex))
                LocalDict.ServerIdDict.Remove(fileInfoRow.ServerIndex);

            var rows = fileInfoDataTable.Select("FileIndex='" + fileInfoRow.FileIndex + "'");
            foreach (DB.FileInfoRow row in rows)
            {
                row.Delete();
                //Adapter.FileInfoTableAdapter.Update(row);
            }
            //fileInfoRow.Delete();
            // 更新数据库
            //Adapter.FileInfoTableAdapter.Update(fileInfoRow);
            Adapter.FileInfoTableAdapter.Update(fileInfoDataTable);
            fileInfoDataTable.AcceptChanges();
            //SyncDB.FileInfo.AcceptChanges();
        }

        public void DeleteClientItemFolder(DB.FileInfoRow fileInfoRow)
        {
            var fileInfoDataTable = Adapter.FileInfoTableAdapter.GetData();
            var rows = fileInfoDataTable.Select("FileIndex='" + fileInfoRow.FileIndex + "'");

            if (rows.Length > 0)
            {
                var fileRow = rows[0] as DB.FileInfoRow;
                var rowsToRemove = GetRecursiveInfoRows(fileRow, fileInfoDataTable);

                foreach (var row in rowsToRemove)
                {
                    // 更新字典
                    if (LocalDict.FileIndexDict.ContainsKey(row.FileIndex))
                        LocalDict.FileIndexDict.Remove(row.FileIndex);
                    if (!row.IsServerIndexNull() && LocalDict.ServerIdDict.ContainsKey(row.ServerIndex))
                        LocalDict.ServerIdDict.Remove(row.ServerIndex);

                    // 删除数据库行
                    row.Delete();
                }

                // 更新数据库
                //Adapter.FileInfoTableAdapter.Update(SyncDB.FileInfo);
                //SyncDB.FileInfo.AcceptChanges();
                Adapter.FileInfoTableAdapter.Update(fileInfoDataTable);
                fileInfoDataTable.AcceptChanges();
            }
        }

        /// <summary>
        /// 更新文件在数据库中的信息
        /// </summary>
        /// <param name="ci"></param>
        /// <param name="si"></param>
        public void UpdateClientItem(ClientItem ci, ServerItem si, DB.FileInfoRow fileInfoRow)
        {
            var fileInfoDataTable = Adapter.FileInfoTableAdapter.GetData();
            var rows = fileInfoDataTable.Select("FileIndex='" + fileInfoRow.FileIndex + "'");

            if (rows.Length > 0)
            {
                var row = rows[0] as DB.FileInfoRow;

                row.CacheFolder = fileInfoRow.CacheFolder;

                if (ci.FileIndex != row.FileIndex)
                    row.FileIndex = ci.FileIndex;

                row.Hash = ci.Hash;
                row.ItemSize = (int)ci.Size;
                row.LastWrite = ci.BasicInfo.LastWriteTime;

                if (si != null)
                {
                    row.ServerVersion = si.Version;
                    row.ServerIndex = si.Id;
                }

                Adapter.FileInfoTableAdapter.Update(row);
                fileInfoDataTable.AcceptChanges();

                UpdateLocalDict(row);
            }
        }

        private void UpdateLocalDict(DB.FileInfoRow fileInfoRow)
        {
            var fileInfoDataTable = Adapter.FileInfoTableAdapter.GetData();
            var rows = fileInfoDataTable.Select("FileIndex='" + fileInfoRow.FileIndex + "'");

            if (rows.Length > 0)
            {
                var row = rows[0] as DB.FileInfoRow;

                LocalDict.FileIndexDict.Remove(row.FileIndex);
                LocalDict.FileIndexDict[row.FileIndex] = row;

                LocalDict.ServerIdDict.Remove(row.ServerIndex);
                LocalDict.ServerIdDict.Add(row.ServerIndex, row);

                LocalDict.ServerUniqueId.Remove(row.Uid);
                LocalDict.ServerUniqueId.Add(row.Uid, row);
            }
        }

        /// <summary>
        /// 递归获取目录及目录下的信息
        /// </summary>
        /// <param name="folderRow"></param>
        /// <returns></returns>
        public List<DB.FileInfoRow> GetRecursiveInfoRows(DB.FileInfoRow folderRow, DB.FileInfoDataTable fileInfoTable)
        {
            List<DB.FileInfoRow> result = new List<DB.FileInfoRow>();
            result.Add(folderRow);
            //var subRows = SyncDB.FileInfo.Select("ParentFolder='" + folderRow.FileIndex + "'");
            var subRows = fileInfoTable.Select("ParentFolder='" + folderRow.FileIndex + "'");
            foreach (DB.FileInfoRow infoRow in subRows)
            {
                if (infoRow.ItemType == 0)
                    result.Add(infoRow);
                else
                    result.AddRange(GetRecursiveInfoRows(infoRow, fileInfoTable));
            }
            return result;
        }

        public IEnumerable<UploadedFile> GetUploadedFiles()
        {
            try
            {
                Adapter.UploadingFileDetailTableAdapter.Fill(SyncDB.UploadingFileDetail);
                UploadedFile uploedFile = new UploadedFile(SyncDB.UploadingFileDetail);

                return uploedFile.UploadedFilesToList();
            }
            catch (Exception ex)
            {
                //生成文件列表索引字典失败
                this.AddErrorInfo("11129", "获取已上传文件信息失败", ex.ToString(), null);
                throw new DBException("FileDetail数据表中存在异常.", ex);
            }
        }

        /// <summary>
        /// 获取本地文件记录字典
        /// </summary>
        /// <returns></returns>
        public LocalFilesDict GetFilesDict()
        {
            this.AddLogMessage("开始生成文件索引字典.");
            try
            {
                Adapter.FileInfoTableAdapter.Fill(SyncDB.FileInfo);
                return new LocalFilesDict(SyncDB.FileInfo);
            }
            catch (Exception ex)
            {
                //生成文件列表索引字典失败
                this.AddErrorInfo("1112", "生成文件列表索引字典失败", ex.ToString(), null);
                throw new DBException("FileInfo数据表中存在异常.", ex);
            }
        }
    }
}
