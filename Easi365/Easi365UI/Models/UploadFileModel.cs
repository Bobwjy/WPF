using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM = Caliburn.Micro;
using System.Collections;
using ClientLib;
using ClientLib.Entities;
using Easi365UI.Windows;
using System.IO;
using System.Collections.ObjectModel;

namespace Easi365UI.Models
{
    public class UploadFileModel : CM.PropertyChangedBase
    {
        ClientItem _ci;
        string _root;

        public ClientItem ClientItem { get { return _ci; } }

        public UploadState UploadState { get; private set; }

        public UploadFileModel()
        {

        }

        public UploadFileModel(ClientItem ci)
        {
            _ci = ci;
            UploadState = new UploadState();
        }

        public UploadFileModel(ClientItem ci, string rootPath)
            : this(ci)
        {
            _root = rootPath;
        }

        public string Name
        {
            get
            {
                if (TempFile != null && !string.IsNullOrEmpty(TempFile.NewName))
                    return TempFile.NewName;
                return _ci.BasicInfo.Name;
            }
        }
        public string FullPath { get { return _ci.BasicInfo.FullName; } }
        public bool IsDirectory { get { return _ci.ItemType == FileOrFolderType.Folder; } }
        public bool Uploaded { get; set; }

        public TempFile TempFile { get; set; }
        public string CreatedPath
        {
            get
            {
                if (TempFile != null && !string.IsNullOrEmpty(TempFile.NewName))
                    return string.Format("{0}{1}", Path.DirectorySeparatorChar, TempFile.NewName);
                return FullPath.Substring(_root.Length);
            }
        }

        private string _status = string.Empty;
        public string Status
        {
            get
            {
                if (string.IsNullOrEmpty(_status))
                {
                    if (IsDirectory)
                    {
                        _status = "等待创建";
                    }
                    else
                    {
                        _status = "等待上传";
                    }
                }

                return _status;
            }
            set { _status = value; NotifyOfPropertyChange(() => Status); }
        }

        public string Description { get; set; }
    }



    //public enum UploadStatus
    //{
    //    Waiting,
    //    Uploading,
    //    Uploaded
    //}

    public class FileDropHandler
    {
        UploadingItem _uploadingItem = new UploadingItem();
        TempFile[] _dropFiles;
        string _relativePath;

        SyncManager _sm;

        string _rootPath = "";

        public UploadingItem UploadingItems
        {
            get { return _uploadingItem; }
        }

        public ArrayList al = new ArrayList();

        public delegate void UploadCompleteHandler();
        public event UploadCompleteHandler UploadCompleted;

        public FileDropHandler(SyncManager sm, TempFile[] dropfiles, string relUrl)
        {
            this._dropFiles = dropfiles;
            this._sm = sm;
            this._relativePath = relUrl;

            AnalyzeDropFile(this._dropFiles);
        }

        public async Task Handle(ServerSide server, Action<UploadFileModel, ServerItem> callback, Action refreshList = null)
        {
            //Task task = new Task(() =>
            //{
            foreach (UploadFileModel file in UploadingItems)
            {
                //file.Status = "正在上传...";
                if (file.IsDirectory)
                {
                    await server.CreateNewFolder(null, this._relativePath + file.CreatedPath.TrimStart('\\'), b =>
                     {
                         // callback(file);
                     });
                }
                else
                {
                    //server.CreateNewFile(file.ClientItem,null, this._relativePath + file.CreatedPath, b =>
                    //{
                    //    callback(file);
                    //});
                    file.Status = "正在上传";
                    FileInfo fileInfo = new FileInfo(file.ClientItem.BasicInfo.FullName);
                    string extesion = fileInfo.Extension;
                    if (extesion == ".exe" || extesion == ".dll")
                    {
                        file.Status = "安全限制无法上传";
                    }
                    else
                    {
                        await server.CreateNewFileAsync(file.ClientItem,
                            null,
                            file.UploadState,
                            this._relativePath + file.CreatedPath,
                            (b, si) => { callback(file, si); });
                    }
                }
            }
            if (UploadCompleted != null)
                UploadCompleted();
            //if (refreshList != null)
            //    refreshList();
            //});
            //task.Start();
        }

        private void AnalyzeDropFile(TempFile[] files)
        {
            //1. 是否为单个文件
            if (files != null && files.Length > 0)
            {
                foreach (var tempFile in files)
                {
                    DirectoryInfo di = new DirectoryInfo(tempFile.OriginalPath);
                    if (di.Exists)
                    {
                        // DirectoryInfo di = new DirectoryInfo(path);
                        string parentPath = di.Parent.FullName;
                        GetAllDirList(di, parentPath);

                    }
                    else
                    {
                        FileInfo fi = new FileInfo(tempFile.OriginalPath);
                        var model = new UploadFileModel(new ClientItem(fi), fi.Directory.FullName);
                        model.TempFile = tempFile;
                        _uploadingItem.Add(model);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <param name="rootPath"></param>
        private void GetAllDirList(DirectoryInfo di, string rootPath)
        {
            //创建目录 操作不在上传文件窗体显示
            _uploadingItem.Add(new UploadFileModel(new ClientItem(di), rootPath));

            var files = di.GetFiles();
            foreach (FileInfo fileInfo in files)
                _uploadingItem.Add(new UploadFileModel(new ClientItem(fileInfo), rootPath));

            DirectoryInfo[] diA = di.GetDirectories();

            for (int i = 0; i < diA.Length; i++)
                GetAllDirList(diA[i], rootPath);
        }
    }

    public class TempFile
    {
        public TempFile(string path)
        {
            this.OriginalPath = path;
        }

        public TempFile(string path, ObservableCollection<IEntryModel> files)
            : this(path)
        {
            this.BuildFileName(files);
        }

        public string OriginalPath { get; set; }

        public string Name { get; set; }
        public string NewName { get; set; }

        private void BuildFileName(ObservableCollection<IEntryModel> files)
        {
            DirectoryInfo di = new DirectoryInfo(this.OriginalPath);
            if (!di.Exists)
            {
                string name = Path.GetFileName(this.OriginalPath);

                bool flag = true;
                int index = 1;
                do
                {
                    if (files.Where(m => m.Name == name).Count() == 0)
                    {
                        break;
                    }
                    else
                    {
                        name = string.Format("{0}({1}){2}", Path.GetFileNameWithoutExtension(this.OriginalPath), index, Path.GetExtension(name));
                        index++;
                    }
                } while (flag);

                if (index != 1)
                    this.NewName = name;
                this.Name = name;
            }
        }
    }
}

