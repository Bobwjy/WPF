using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Timers;
using ST = System.Threading;
using ClientLib.Entities;
using System.Security.Permissions;
using System.Diagnostics;
using ClientLib.Utilities;

namespace ClientLib.Core
{
    public class FileWatcher
    {
        //忽略监控的文件类型
        List<string> _ignoreFiles = new List<string>() 
        {
            ".tmp",".xml"
        };

        //string[] _officeFilters = { "*.pptx", "*.doc", "*.docx", "*.xls", "*.xlsx", "*.ppt" };
        string[] _officeFilters = { ".pptx", ".doc", ".docx", ".xls", ".xlsx", ".ppt" };
        string[] _filters = { "*.txt" };

        List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();

        Dictionary<string, FileSystemWatcher> _monitors = new Dictionary<string, FileSystemWatcher>();

        private static object _locker = new object();
        private static bool _isBusy = false;

        FileSystemWatcher _watcher;
        Timer _taskTimer;
        string _path;

        public FileWatcher()
        {
        }

        public FileWatcher(string path)
        {
            this._path = path;
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

        //定时器方法 检查TaskInfoList并上传文件
        void _taskTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!TryLock())
                return;

            //遍历所有任务 并启动
            try
            {
                ST.Monitor.Enter(CoreManager.TaskManager.TaskInfosLock);
                foreach (TaskInfo task in CoreManager.TaskManager.TaskInfos)
                    CoreManager.TaskManager.StartTask(task);
                ST.Monitor.Exit(CoreManager.TaskManager.TaskInfosLock);
            }
            catch (Exception ex)
            {
                Logging.Add("自动保存文件错误.", ex);
            }
            finally
            {
                ReleaseLock();
            }
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void SetFileMonitor(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
                throw new ArgumentException("monitor file path");

            string fileInfoPath = fileInfo.DirectoryName + "\\Info.xml";
            var localFileInfo = XmlHelper.XmlDeserializeFromFile<LocalFileInfo>(fileInfoPath, Encoding.UTF8);

            if (!_monitors.Keys.Contains(localFileInfo.Id))
            {
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = fileInfo.DirectoryName;
                watcher.IncludeSubdirectories = false;

                if (_officeFilters.Contains(fileInfo.Extension))
                    watcher.NotifyFilter = NotifyFilters.CreationTime;
                else
                    watcher.NotifyFilter = NotifyFilters.Size;

                watcher.Changed += _monitor_Changed;
                watcher.EnableRaisingEvents = true;

                _monitors.Add(localFileInfo.Id, watcher);
            }
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void StartMonitoring()
        {
            //_watcher = new FileSystemWatcher();
            //_watcher.Path = _path;
            //_watcher.IncludeSubdirectories = true;
            //_watcher.EnableRaisingEvents = true;
            ////office文档保存的时候会修改属性
            //_watcher.NotifyFilter = NotifyFilters.Attributes |  NotifyFilters.Size;
            //_watcher.Changed += _watcher_Changed;

            //foreach (string f in _filters)
            //{
            //    FileSystemWatcher watcher = new FileSystemWatcher();
            //    watcher.Path = _path;
            //    watcher.IncludeSubdirectories = true;
            //    watcher.Filter = f;
            //    watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.Size | NotifyFilters.LastWrite;
            //    watcher.Changed += _watcher_Changed;
            //    watcher.EnableRaisingEvents = true;
            //    _watchers.Add(watcher);
            //}

            _taskTimer = new Timer(CoreManager.ConfigManager.Settings.AutoSaveFileToServerTick);

            _taskTimer.Elapsed += _taskTimer_Elapsed;
            _taskTimer.Enabled = true;
            _taskTimer.AutoReset = true;
        }

        void _monitor_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                var filePath = e.FullPath;
                Debug.WriteLine("发生变化的文件：" + filePath);


                FileInfo fileInfo = new System.IO.FileInfo(filePath);
                if (!fileInfo.Exists)
                    return;

                string fileInfoPath = fileInfo.DirectoryName + "\\Info.xml";
                var localFileInfo = XmlHelper.XmlDeserializeFromFile<LocalFileInfo>(fileInfoPath, Encoding.UTF8);

                //监控office文档 当文档保存时会监控到文档的临时文件 需要过滤临时文件
                if (!string.Equals(localFileInfo.FileName, fileInfo.Name, StringComparison.InvariantCultureIgnoreCase))
                    return;

                var changedFile = fileInfo.Directory
                    .GetFiles()
                    .Where(f => f.Name.Equals(localFileInfo.FileName,StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

                if (changedFile != null)
                {
                    Debug.WriteLine("监控到可以上传的文件："+changedFile.FullName);
                    //CoreManager.TaskManager.AddTask(changedFile.FullName);
                    //当前正在编辑的文档被占用不能被上传
                    //但是可以复制文件（拷贝文件副本，并上传，上传完成后需要删除临时文件）
                    var tempFilePath = changedFile.Directory.FullName + "\\" + Guid.NewGuid().ToString() + ".tmp";
                    File.Copy(changedFile.FullName, tempFilePath, true);
                    CoreManager.TaskManager.AddTask(changedFile.FullName, tempFilePath);
                }
            }
            catch (Exception ex)
            {
                Logging.Add("监控文件变化错误.", ex);
            }
        }

        void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                var filePath = e.FullPath;
                Debug.WriteLine("监测到变化的文件：" + filePath);

                DirectoryInfo dir = new System.IO.DirectoryInfo(filePath);
                if (dir.Exists)
                    return;

                FileInfo fileInfo = new System.IO.FileInfo(filePath);
                var changedFile = fileInfo.Directory
                    .GetFiles()
                    .Where(f => ((f.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden) &&
                        !_ignoreFiles.Contains(f.Extension))
                    .FirstOrDefault();

                if (changedFile != null)
                {
                    //过滤不需要上传的文件
                    if (_ignoreFiles.Contains(changedFile.Extension))
                    {
                        Debug.WriteLine("忽略文件：" + changedFile.FullName);
                        return;
                    }

                    //CoreManager.TaskManager.AddTask(changedFile.FullName);
                    //当前正在编辑的文档被占用不能被上传
                    //但是可以复制文件（拷贝文件副本，并上传，上传完成后需要删除临时文件）
                    var tempFilePath = changedFile.Directory.FullName + "\\" + Guid.NewGuid().ToString() + ".tmp";
                    File.Copy(changedFile.FullName, tempFilePath, true);
                    CoreManager.TaskManager.AddTask(changedFile.FullName, tempFilePath);
                }
            }
            catch (Exception ex)
            {
                Logging.Add("监控文件变化错误.", ex);
            }
        }

        public void StopMonitoring()
        {
            //_watcher.EnableRaisingEvents = false;
            if (_monitors.Count > 0)
                foreach (var w in _monitors)
                    w.Value.EnableRaisingEvents = false;

            if (_watchers.Count > 0)
                foreach (FileSystemWatcher watcher in _watchers)
                    watcher.EnableRaisingEvents = false;

            _taskTimer.Enabled = false;
            _taskTimer.AutoReset = false;
        }
    }
}
