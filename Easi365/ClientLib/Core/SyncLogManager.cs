using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace ClientLib.Core
{
    public class SyncLogManager
    {
        private static object _locker = new object();
        private static bool _isBusy = false;

        LocalDB _db;
        Timer _taskTimer;

        public SyncLogManager()
        {
            _db = new LocalDB(CoreManager.ConfigManager.Settings.DbPath + Path.DirectorySeparatorChar + "EasiDB.mdb");
        }

        private bool TryLock()
        {
            lock (_locker)
            {
                if (_isBusy)
                    return false;
                _isBusy = true;
                return true;
            }
        }

        /// <summary>
        /// 工作完毕，释放锁
        /// </summary>
        private void ReleaseLock()
        {
            lock (_locker)
            {
                _isBusy = false;
            }
        }

        public void StartSyncLogTask()
        {
            _taskTimer = new Timer(10000);

            _taskTimer.Elapsed += _taskTimer_Elapsed;
            _taskTimer.Enabled = true;
            _taskTimer.AutoReset = true; 
        }

        public void StopSyncLogTask()
        {
            _taskTimer.Enabled = false;
            _taskTimer.AutoReset = false;

            //_taskTimer.Dispose();
            //_taskTimer = null;
        }

        void _taskTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!TryLock())
                return;

            try
            {
                Logging.AddInfo("开始同步本地数据库日志.");
                var syncLogToken = _db.SyncLogToken;
                var syncLogs = _db.GetSyncLogsByID(syncLogToken);
                ClientLib.Services.SyncLogService logSer = new Services.SyncLogService();
                logSer.SyncLog(syncLogs);

                var syncLastItem = syncLogs
                    .AsQueryable()
                    .OrderByDescending(s => s.ID)
                    .FirstOrDefault();
                //将这次同步记录的最大ID 设置为SyncLogToken
                if (syncLastItem != null)
                    _db.SyncLogToken = syncLastItem.ID;

                Logging.AddInfo(string.Format("同步本地数据库日志成功.共同步{0}条记录.",syncLogs.Count()));
            }
            catch (Exception ex)
            {
                Logging.Add("同步日志发生错误.", ex);
            }
            finally
            {
                ReleaseLock();
            }
        }
    }
}
