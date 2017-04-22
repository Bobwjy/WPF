using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Easi365DB.DBTableAdapters
{
    public partial class TableAdapterManager
    {
        public void InitAdapter(string connectionString)
        {
            // 日志表适配器
            this.LogTableAdapter = new LogTableAdapter();
            this.LogTableAdapter.Connection.ConnectionString = connectionString;

            // 错误表
            this.ErrorsTableAdapter = new ErrorsTableAdapter();
            this.ErrorsTableAdapter.Connection.ConnectionString = connectionString;

            // 文件信息表
            this.FileInfoTableAdapter = new FileInfoTableAdapter();
            this.FileInfoTableAdapter.Connection.ConnectionString = connectionString;

            // Action挂起表
            this.SuspendOperationsTableAdapter = new SuspendOperationsTableAdapter();
            this.SuspendOperationsTableAdapter.Connection.ConnectionString = connectionString;

            //上传文件历史表
            this.UploadingFileDetailTableAdapter = new UploadingFileDetailTableAdapter();
            this.UploadingFileDetailTableAdapter.Connection.ConnectionString = connectionString;

            //日志同步token表
            this.SyncLogInfoTableAdapter = new SyncLogInfoTableAdapter();
            this.SyncLogInfoTableAdapter.Connection.ConnectionString = connectionString;

            //日志表
            this.SyncLogTableAdapter = new SyncLogTableAdapter();
            this.SyncLogTableAdapter.Connection.ConnectionString = connectionString;
        }
    }
}
