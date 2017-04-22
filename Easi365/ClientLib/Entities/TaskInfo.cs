using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ClientLib.SyncActions;
using SP = Microsoft.SharePoint.Client;
using IO = System.IO;
using ClientLib.Core;
using System.Diagnostics;
using ClientLib.Utilities;
using System.IO;

namespace ClientLib.Entities
{
    /// <summary>
    /// 上传状态
    /// </summary>
    public enum UploadStatus
    {
        /// <summary>
        /// 等待
        /// </summary>
        Waiting = 0,
        /// <summary>
        /// 正在上传
        /// </summary>
        Uploading = 1,
        /// <summary>
        /// 上传完成
        /// </summary>
        Completed = 2,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 3
    }

    [Serializable]
    public class TaskInfo : IXmlSerializable
    {
        private ClientItem _ci;

        public TaskInfo(string filePath)
        {
            _ci = new ClientItem(new System.IO.FileInfo(filePath));
            this.FileIndex = _ci.FileIndex;
            this.PathHash = _ci.PathHash;
            this.FullPath = _ci.BasicInfo.FullName;

            UploadState = new UploadState();
        }

        public TaskInfo(string filePath, string tempFilePath)
            : this(filePath)
        {
            this.TempFilePath = tempFilePath;
        }

        private Guid _taskId;

        public Guid TaskId
        {
            get
            {
                if (_taskId == null)
                    _taskId = Guid.NewGuid();
                return _taskId;
            }
            set
            {
                _taskId = value;
            }
        }

        /// <summary>
        /// 文件在磁盘分区上的唯一标识（NTFS上唯一，FAT分区可能会改变）
        /// </summary>
        public string FileIndex { get; set; }

        /// <summary>
        /// 文件路径的唯一标识
        /// </summary>
        public string PathHash { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 临时文件路径,office文件打开状态不能读取并上传文件
        /// 复制原文件并重命名，上传此文件到服务器
        /// </summary>
        public string TempFilePath { get; set; }

        /// <summary>
        /// 文件上传状态
        /// </summary>
        public UploadStatus Status { get; set; }

        /// <summary>
        /// 包含上传进度 当前上传量 和 CancellationTokenSource
        /// </summary>
        public UploadState UploadState { get; set; }

        public async void Start(ServerSide server)
        {
            try
            {
                string fileInfoPath = (this._ci.BasicInfo as FileInfo).DirectoryName + "\\Info.xml";
                var fileInfo = XmlHelper.XmlDeserializeFromFile<LocalFileInfo>(fileInfoPath, Encoding.UTF8);
                var filePath = fileInfo.ServerRelativePath;

                if (!fileInfo.IsNormalFile)
                {
                    var host = (new Uri(server.ClientCtx.Url)).GetWebHost();
                    filePath = host + filePath;
                }

                // string relativePath  = server.DB.LocalDict.FilePathHashDict[this.PathHash].ServerRelativeUrl;
                UpdateRemoteFile updateFileService = new UpdateRemoteFile(this._ci, filePath);
                updateFileService.UseAbsolutePath = !fileInfo.IsNormalFile;

                await updateFileService.UpdateFile(server, this.TempFilePath, this.UploadState, (b, s) =>
                {
                    if (!b)
                        Debug.WriteLine("更新文件成功!" + _ci.BasicInfo.Name);
                    else
                        Debug.WriteLine("更新文件失败!" + _ci.BasicInfo.FullName);
                });
            }
            catch (Exception ex)
            {
                Logging.Add(string.Format("更新文件失败.文件:{0}. 信息:{1}", _ci.BasicInfo.FullName, ex.Message), ex);
                Debug.WriteLine(string.Format("更新文件失败.文件:{0}. 信息:{1}", _ci.BasicInfo.FullName, ex.Message));
            }
            finally
            {
                //上传结束后删除临时文件
                DeleteTempFile();
            }
        }

        /// <summary>
        /// 停止任务
        /// </summary>
        public void Stop()
        {
            try
            {
                this.UploadState.CancellationToken.Cancel(true);

                DeleteTempFile();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("取消上传任务失败,文件:{0}. 信息;{1}",this.FullPath,ex.Message));
            }
        }

        /// <summary>
        /// 删除临时文件
        /// </summary>
        private void DeleteTempFile()
        {
            try
            {
                System.IO.FileInfo tempFile = new IO.FileInfo(this.TempFilePath);
                if (tempFile.Exists)
                    tempFile.Delete();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("删除临时 文件:{0} 错误.信息: {1}",this.TempFilePath,ex.Message));
            }
        }


        #region IXmlSerializable成员
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            var s = new XmlSerializer(typeof(string));

            if (reader.IsEmptyElement || !reader.Read())
            {
                return;
            }
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("TaskInfo");

                reader.ReadStartElement("TaskId");
                TaskId = new Guid((string)s.Deserialize(reader));
                reader.ReadEndElement();

                reader.ReadStartElement("FileIndex");
                FileIndex = (string)s.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("PathHash");
                PathHash = (string)s.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("FullPath");
                FullPath = (string)s.Deserialize(reader);
                reader.ReadEndElement();
            }

            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            var s = new XmlSerializer(typeof(string));

            writer.WriteStartElement("TaskInfo");

            //TaskId
            writer.WriteStartElement("TaskId");
            s.Serialize(writer, TaskId.ToString());
            writer.WriteEndElement();

            //FileIndex
            writer.WriteStartElement("FileIndex");
            s.Serialize(writer, FileIndex);
            writer.WriteEndElement();

            //PathHash
            writer.WriteStartElement("PathHash");
            s.Serialize(writer, PathHash);
            writer.WriteEndElement();

            //FullPath
            writer.WriteStartElement("FullPath");
            s.Serialize(writer, FullPath);
            writer.WriteEndElement();

            writer.WriteEndAttribute();
        }

        #endregion
    }
}
