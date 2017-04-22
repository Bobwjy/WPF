using ClientLib.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading;
using System.Diagnostics;

namespace ClientLib.Core
{
    public class TaskManager
    {
        public TaskManager()
        {

        }

        //所有任务
        public List<TaskInfo> TaskInfos = new List<TaskInfo>();
        /// <summary>
        /// 服务器端实例
        /// </summary>
        public ServerSide Server { get; set; }

        public void AddTask(string filePath, string tempPath)
        {
            TaskInfo task = new TaskInfo(filePath, tempPath);
            Monitor.Enter(TaskInfosLock);
            //首先检查是否有当前文件的上传任务
            //如果当前任务不是 正在上传 首先将任务从集合移除 然后再添加 
            //用户多次触发保存操作 只需要将最新的文件上传即可
            var preTask = TaskInfos
               .Where(p => p.PathHash == task.PathHash)
               .FirstOrDefault();
            if (preTask != null)
            {
                //if (preTask.Status != UploadStatus.Uploading)
                //    TaskInfos.Remove(preTask);
                //else
                //    preTask.Stop();
                preTask.Stop();
                System.Threading.Thread.Sleep(50);

                TaskInfos.Remove(preTask);
            }

            TaskInfos.Add(task);
            Monitor.Exit(TaskInfosLock);
        }

        public void DeleteTask(TaskInfo task)
        {
            if (task.Status != UploadStatus.Completed)
                return;

            Monitor.Enter(TaskInfosLock);
            TaskInfos.Remove(task);
            Monitor.Exit(TaskInfosLock);
        }

        public void StartTask(TaskInfo task)
        {
            if (task.Status == UploadStatus.Uploading
                || task.Status == UploadStatus.Completed
                || task.Status == UploadStatus.Error)
                return;

            ThreadPool.QueueUserWorkItem(new WaitCallback(s =>
            {
                try
                {
                    Debug.WriteLine("正在上传文件:" + task.FullPath);
                    task.Status = UploadStatus.Uploading;
                    task.Start(Server);
                    task.Status = UploadStatus.Completed;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("上传文件错误:" + ex.Message);
                    Logging.Add("上传并更新服务器文件出现错误.文件路径:" + task.FullPath, ex);
                    task.Status = UploadStatus.Error;
                }
            }));
        }

        //TaskInfos对象的全局锁
        public object TaskInfosLock = new object();

        private object saveTaskLock = new object();
        public void SaveAllTasks()
        {
            lock (saveTaskLock)
            {
                //序列化至内存流
                using (MemoryStream ms = new MemoryStream())
                {
                    try
                    {
                        XmlSerializer formatter = new XmlSerializer(typeof(List<TaskInfo>));
                        formatter.Serialize(ms, TaskInfos);
                        //将内存流复制到文件
                        using (FileStream fs = new FileStream(Path.Combine(CoreManager.StartupPath, "Task.xml"), FileMode.Create))
                        {
                            ms.Position = 0;
                            byte[] buffer = new byte[500 * 1024];
                            int read = 0;
                            read = ms.Read(buffer, 0, buffer.Length);
                            while (read > 0)
                            {
                                fs.Write(buffer, 0, read);
                                read = ms.Read(buffer, 0, buffer.Length);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Add(ex);
                    }
                }
                //保证TaskInfos对象不会被意外回收
                GC.KeepAlive(TaskInfos);
            }
        }

        public void LoadAllTask()
        {
            //取得文件路径名称
            string path = Path.Combine(CoreManager.StartupPath, "Task.xml");
            //如果文件存在
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    try
                    {
                        XmlSerializer formatter = new XmlSerializer(typeof(List<TaskInfo>));
                        TaskInfos = (List<TaskInfo>)formatter.Deserialize(fs);
                    }
                    catch (Exception ex)
                    {
                        Logging.Add(ex);
                    }
                }
            }
        }
    }
}
