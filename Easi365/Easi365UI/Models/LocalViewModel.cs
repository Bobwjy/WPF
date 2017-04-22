using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ClientLib.Core;
using ClientLib.Entities;
using Easi365UI.Util;
using System.Windows.Threading;
using System.Threading;

namespace Easi365UI.Models
{
    public class LocalViewModel : INotifyPropertyChanged
    {
        public static string RootFolder = CoreManager.ConfigManager.Settings.LocalFolderPath;

        Dispatcher _dispatcher;
        public LocalViewModel(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;

            this.Directory = RootFolder;
            if (!System.IO.Directory.Exists(Directory))
                System.IO.Directory.CreateDirectory(Directory);
            this.localItems = new ObservableCollection<LocalFile>();

            //导航
            this.NavigationModel = new EasiNavigationModel(RootFolder);
        }

        private string directory;
        public string Directory
        {
            get { return directory; }
            set { directory = value; OnPropertyChanged("Directory"); }
        }

        public LocalFile CurrentModel
        {
            get
            {
                return new LocalFile(new DirectoryInfo(Directory));
            }
        }

        private ObservableCollection<LocalFile> localItems;
        public ObservableCollection<LocalFile> LocalItems
        {
            get { return localItems; }
            set { localItems = value; OnPropertyChanged("Items"); }
        }

        private EasiNavigationModel navigationModel;
        public EasiNavigationModel NavigationModel
        {
            get { return navigationModel; }
            set { navigationModel = value; OnPropertyChanged("NavigationModel"); }
        }

        public ICommand GoCommand
        {
            get
            {
                return new LocalFileCommand(LoadItems);
            }
        }

        public ICommand UpCommand
        {
            get
            {
                return new LocalFileCommand(method =>
                {
                    if (CurrentModel != null &&
                        new DirectoryInfo(CurrentModel.Info.FullName).FullName != CoreManager.ConfigManager.Settings.LocalFolderPath)
                        LoadItems(CurrentModel.GoUp());
                });
            }
        }

        public static RoutedUICommand OpenCommand = new RoutedUICommand();
        public static RoutedUICommand RefreshCommand = new RoutedUICommand();
        public static RoutedUICommand CutCommand = new RoutedUICommand();
        public static RoutedUICommand CopyCommand = new RoutedUICommand();
        public static RoutedUICommand PasteCommand = new RoutedUICommand();
        public static RoutedUICommand DeleteCommand = new RoutedUICommand();
        public static RoutedUICommand RenameCommand = new RoutedUICommand();
        public static RoutedUICommand PropertyCommand = new RoutedUICommand();

        public static RoutedUICommand NavigationBackward = new RoutedUICommand();
        public static RoutedUICommand NavigationForward = new RoutedUICommand();

        public static RoutedUICommand CompanyNavigationBackward = new RoutedUICommand();
        public static RoutedUICommand CompanyNavigationForward = new RoutedUICommand();


        public static RoutedUICommand LocalNavigationBackward = new RoutedUICommand();
        public static RoutedUICommand LocalNavigationForward = new RoutedUICommand();

        public static RoutedUICommand SendFileCommand = new RoutedUICommand();
        public static RoutedUICommand SharingCommand = new RoutedUICommand();
        public static RoutedUICommand PersonRenameCommand = new RoutedUICommand();

        //取消正在下载的文件
        public static RoutedUICommand CancelDownloadingCommand = new RoutedUICommand();
        //下载文件（当前版本不支持文件夹下载）
        public static RoutedUICommand DownloadFileCommand = new RoutedUICommand();
        //托盘图标单击打开主窗体路由事件
        public static RoutedUICommand OpenMainWindowCommand = new RoutedUICommand();

        //共享给我的文档
        public static RoutedUICommand SharedWithMeOpenCommand = new RoutedUICommand();
        public static RoutedUICommand SharedWithMeDownloadCommand = new RoutedUICommand();
        public static RoutedUICommand SharedWithMeFollowCommand = new RoutedUICommand();
        public static RoutedUICommand SharedWithMeRefreshCommand = new RoutedUICommand();

        //关注文档
        public static RoutedUICommand FollowCommand = new RoutedUICommand();

        //预览文档按钮命令
        public static RoutedUICommand ViewDocCommand = new RoutedUICommand();

        //查看版本历史记录
        public static RoutedUICommand ViewVersionHistoryCommand = new RoutedUICommand();

        public void LoadItems(object p)
        {
            Task.Factory.StartNew(delegate()
            {
                if (p != null && p is string)
                    Directory = (string)p;

                if (System.IO.Directory.Exists(Directory))
                {
                    _dispatcher.Invoke(new Action(() =>
                    {
                        DirectoryInfo dir = new DirectoryInfo(Directory);
                        LocalItems.Clear();
                        try
                        {
                            var list = dir.EnumerateFileSystemInfos().OrderByDescending(m => (m.Attributes & FileAttributes.Directory) == FileAttributes.Directory);
                            foreach (FileSystemInfo info in list)
                            {
                                if ((info.Attributes & FileAttributes.Directory) == FileAttributes.Directory ||
                                    (info.Attributes & FileAttributes.Archive) == FileAttributes.Archive ||
                                    (info.Attributes & FileAttributes.Normal) == FileAttributes.Normal)
                                    LocalItems.Add(new LocalFile(info));
                            }

                            NavigationModel.Refresh(Directory);
                        }
                        catch (Exception ex)
                        {
                            Logging.Add("本地文件加载异常", ex);
                        }
                    }));
                }
            });
        }

        public void Rename(string oldPath, FileSystemInfo info)
        {
            var index = -1;
            var oldFile = this.LocalItems.Where(m => m.FullName == oldPath).FirstOrDefault();
            if (oldFile != null)
            {
                index = this.LocalItems.IndexOf(oldFile);
                this.LocalItems.RemoveAt(index);
            }
            if (info != null && index != -1)
                this.LocalItems.Insert(index, new LocalFile(info));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
