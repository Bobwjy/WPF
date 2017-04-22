using ClientLib;
using ClientLib.Core;
using ClientLib.Entities;
using ClientLib.Services;
using ClientLib.Utilities;
using Easi365UI.Extention;
using Easi365UI.Models;
using Easi365UI.Models.SkyDrive;
using Easi365UI.Util;
using Easi365UI.Windows;
using Easi365UI.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification.Interop;
using iTuner;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SP = Microsoft.SharePoint.Client;
using mshtml;
using System.Web;
using System.Text;
using System.Windows.Documents;
using Easi365UI.UserControls;
using Easi365UI.Entities;
using System.Diagnostics;
using Odyssey.Controls;
using System.Windows.Media.Animation;

namespace Easi365UI
{
    public class FolderItem
    {
        public string Folder { get; set; }
        public ImageSource Image { get; set; }

        public FolderItem()
            : base()
        {
            ImageSourceConverter isc = new ImageSourceConverter();
           Image = isc.ConvertFrom("openfolderHS.png") as ImageSource;
           // this.Image = new BitmapImage(new Uri("openfolderHS.png", UriKind.Relative));
        }
    }


    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MaxWindow : EasiWindow, INotifyPropertyChanged
    {
        #region 属性
        string space = "ps";
        SyncManager _sm = null;
        IEntryModel _rootDir;
        //IEntryModel _companyRootDir;//用于公司空间协作空间部门空间
        //IEntryModel _companyRootDirSpecify;//仅用作公司空间
        CancellationTokenSource cts = null; //取消操作的标记 上传或下载文件时使用
        SynchronizationContext sc; //UI线程的上下文
        //ObservableCollection<Department> _orgList = null;
        //List<Email> _emailList = null;
        //EmailService _es = null;
        //CreateWebService _createWebService;

        Stack<IEntryModel> _backwardNavigations = new Stack<IEntryModel>();
        Stack<IEntryModel> _forwardNavigations = new Stack<IEntryModel>();
        //1 ObservableCollection  
        //2 构造函数初始化
        //3 this.DataContext = this;构造函数初始化
        public ObservableCollection<IEntryModel> ListViewItems { get; set; }
        //public ObservableCollection<IEntryModel> CompanyListViewItems { get; set; }
        //public ObservableCollection<IEntryModel> DeptListViewItems { get; set; }
        public LocalViewModel LocalViewModel { get; set; }

        SkyDriveProfile _personalProfile;  //关注的文档 Profile
        ServerSide CurrentServer;

        ViewDocPage viewPage;
        //capad WebBrowser
        // WebBrowser browser;

        //搜索中心 WebBrowser
        //ViewDocPage searchPage;
        //WebBrowser searchBrowser;

        //当前目录
        IEntryModel _currentDir;
        public IEntryModel CurrentDirectory
        {
            get
            {
                return this._currentDir;
            }
            set
            {
                SetCurrentFolder(value);
            }
        }

        public void SetCurrentFolder(IEntryModel em)
        {
            _currentDir = em;
            _currentDir.Profile.ListAsync(CurrentDirectory, ListViewItems);
        }

        public ObservableCollection<UploadFileModel> UploadingFiles
        {
            get
            {
                return App.CurrentApp.UploadingFileDetailPage.UploadingItems;
            }
        }

        public string Space
        {
            get { return this.space; }

            set { SetCurrentRootDir(value); }
        }

        public void SetCurrentRootDir(string s)
        {
            sc = SynchronizationContext.Current;
            cts = new CancellationTokenSource();

            SkyDriveProfile profile = new SkyDriveProfile(CurrentServer, sc, cts);
            _rootDir = SkyDriveItemModel.DefaultSkyDriveItemModel(profile, CurrentServer.RootFolderServerRelativeUrl);
        }
        #endregion

        public MaxWindow()
        {
            InitializeComponent();
        }

        public MaxWindow(SyncManager sm)
            : this()
        {
            _sm = sm;
            ListViewItems = new ObservableCollection<IEntryModel>();
            LocalViewModel = new LocalViewModel(this.Dispatcher);
            sc = SynchronizationContext.Current;
            cts = new CancellationTokenSource();

            SkyDriveProfile profile = new SkyDriveProfile(_sm.PersonalServer, sc, cts);
            SkyDriveProfile profile1 = new SkyDriveProfile(_sm.DepartmentServer, sc, cts);
            SkyDriveProfile profile2 = new SkyDriveProfile(_sm.CompanyServer, sc, cts);

            _rootDir = SkyDriveItemModel.DefaultSkyDriveItemModel(profile, _sm.PersonalServer.RootFolderServerRelativeUrl);
            //profile.AggregateExceptionCatched += new EventHandler<SkyDriveProfile.AggregateExceptionArgs>(HandleAggregateExceptions);

            _personalProfile = profile;


            CurrentDirectory = _rootDir;//个人空间当前目录  add listviewitems    （清空）  然后加载文件目录的文件 文件信息
            //CompanyCurrentDirectory = _companyRootDir;
            this.DataContext = this;

            //点击窗体头部区域拖拽整个窗体移动
            this.ContentHeader.MouseLeftButtonDown += HeaderGrid_MouseLeftButtonDown;

            //初始化listview拖拽选中效果
            SetListViewDragSelection();
        }

        #region 窗体加载事件
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //初始化控件高度
                InitControlHeight();
                //初始界面选项
                InitOptions();

                CurrentServer = _sm.PersonalServer;

                BindDocList();

                

            }
            catch
            {

            }
        }

        private void InitControlHeight()
        {
            double cWidth = this.Width;             //当前面板宽度
            double cHeight = this.Height;           //当前面板高度
            double headerHeight = this.ContentHeader.Height;

            //persSpaceGrid.Height = cHeight - toolCanvas.Height - 138;
            persSpaceGrid.Height = cHeight - toolCanvas.Height;

            FileListView.Height = persSpaceGrid.Height - headerHeight;
            //searchGrid.Height = persSpaceGrid.Height - headerHeight;
        }

        private void InitOptions()
        {
            ChangeView();
        }
        #endregion

        #region 窗体绑定事件
        //双击头部最大化窗体
        private void ContentHeader_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToMaxWindow();
        }

        //表格头部点击事件（排序）
        void HeaderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //计算窗体大小
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitControlHeight();
        }

        private void tbMainWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private void tbExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //关闭日志文件
            try
            {
                Logging.Exit();
                CoreManager.FileWatcher.StopMonitoring();
                this.tb.Dispose();
            }
            catch
            {
            }
            Environment.Exit(0);
        }

        //拖拽选中效果
        ListViewAdorner myAdorner { get; set; }
        System.Windows.Point? myDragStartPoint { get; set; }

        private void SetListViewDragSelection()
        {
            this.SourceInitialized += delegate
            {
                myAdorner = new ListViewAdorner(this.FileListView);
                var adornerLayer = AdornerLayer.GetAdornerLayer(this.FileListView);
                adornerLayer.Add(myAdorner);

                this.FileListView.PreviewMouseDown += (o, e) =>
                {
                    //if (this.FileListView.SelectedItem != null)
                    //    return;

                    TextBlock textBlock = e.OriginalSource as TextBlock;
                    if (textBlock != null && textBlock.Text == "取消")
                        return;

                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        return;

                    if (e.ChangedButton == MouseButton.Left)
                    {
                        myDragStartPoint = e.GetPosition(this.FileListView);

                        //判断鼠标点击位置是否为listview头部
                        if (myDragStartPoint.HasValue)
                        {
                            if (myDragStartPoint.Value.Y < 22)
                                myDragStartPoint = null;
                        }

                        this.FileListView.SelectedItems.Clear();
                    }
                };

                this.FileListView.MouseMove += (o, e) =>
                {
                    if (myDragStartPoint.HasValue)
                    {
                        Rect r = new Rect(myDragStartPoint.Value,
                        e.GetPosition(this.FileListView) - myDragStartPoint.Value);
                        myAdorner.HighlightArea = r;
                        var items = this.FileListView.GetItemAt<SkyDriveItemModel>(r);
                        if (items.Count > 0)
                        {
                            this.FileListView.SelectedItems.Clear();
                            foreach (var i in items)
                                this.FileListView.SelectedItems.Add(i);
                        }
                        else
                            this.FileListView.SelectedItems.Clear();
                    }
                };

                this.FileListView.MouseUp += (o, e) =>
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        myDragStartPoint = null;
                        myAdorner.HighlightArea = new Rect();
                    }
                };

                this.FileListView.MouseLeave += (o, e) =>
                {
                    myDragStartPoint = null;
                    myAdorner.HighlightArea = new Rect();
                };
            };
        }
        #endregion

        #region 右上角按钮点击事件
        //设置按钮点击事件
        private void SettingsBtn_Click_1(object sender, RoutedEventArgs e)
        {
            var contextMenu = new ContextMenu();
            var uploadHistory = new MenuItem();
            uploadHistory.Header = "查看传输记录";
            uploadHistory.Click += uploadHistory_Click;
            contextMenu.Items.Add(uploadHistory);

            contextMenu.Items.Add(new Separator());

            var settingsMenu = new MenuItem();
            settingsMenu.Header = "设置...";
            settingsMenu.Click += settingsMenu_Click;

            contextMenu.Items.Add(settingsMenu);
            contextMenu.IsOpen = true;
        }

        void uploadHistory_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentApp.UploadingFileDetailPage.ShowSelfInOneWindowCenter(this);
        }

        void settingsMenu_Click(object sender, RoutedEventArgs e)
        {
            SystemSettings settingsPage = new SystemSettings();
            ShowDialogWindow(settingsPage);
        }

        void ShowDialogWindow(Window window)
        {
            window.Owner = this;
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            window.ShowDialog();
        }

        //最小化按钮点击事件
        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        //最大化按钮点击事件
        private void MaxBtn_Click(object sender, RoutedEventArgs e)
        {
            ToMaxWindow();
        }

        private void ToMaxWindow()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.MaxBtn.ToolTip = "还原";
                this.WindowState = WindowState.Normal;
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/max.png"));
                this.MaxBtn.Background = brush;
                brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/maxb.png"));
                this.MaxBtn.MyEnterBrush = brush;
                this.MaxBtn.MyMoverBrush = brush;
            }
            else
            {
                this.MaxBtn.ToolTip = "最大化";
                this.WindowState = WindowState.Maximized;
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/maxa.png"));
                this.MaxBtn.Background = brush;
                brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/maxab.png"));
                this.MaxBtn.MyEnterBrush = brush;
                this.MaxBtn.MyMoverBrush = brush;
            }
        }

        //关闭按钮点击事件
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide(); ;

            try
            {
                //if (searchPage != null)
                //{
                //    searchPage.Close();
                //    searchPage = null;
                //}

                if (viewPage != null)
                {
                    viewPage.Close();
                    viewPage = null;
                }
            }
            catch
            {
            }
        }

        //关闭窗体时持久化用户配置config.xml
        private void MaxW_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                CoreManager.ConfigManager.SaveSettings();
            }
        }
        #endregion

        #region 导航栏点击事件
        Stack<IEntryModel> _backwardCompanyNavs = new Stack<IEntryModel>();
        Stack<IEntryModel> _forwardCompanyNavs = new Stack<IEntryModel>();
        string path = "/personal/" + CoreManager.ConfigManager.Settings.CurrentUserName + "/Documents";

        //返回首页
        public void NavHomeButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentDirectory = _rootDir;
            _backwardNavigations.Clear();
            _forwardNavigations.Clear();
            this.file_url.Content = "/";
            this.file_name.Text = null;
        }

        /// <summary>
        /// 前进按钮是否能执行
        /// </summary>
        private void NavForwardCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_forwardNavigations.Count > 0)
                e.CanExecute = true;
        }
        /// <summary>
        /// 前进按钮执行操作
        /// </summary>
        private void NavForwardCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_forwardNavigations.Count > 0)
            {
                var forwardFolderPath = _forwardNavigations.Pop();
                _backwardNavigations.Push(CurrentDirectory);
                CurrentDirectory = forwardFolderPath;
                this.file_name.Text = null;
                this.file_url.Content = CurrentDirectory.FullPath.Replace(path, "");
            }
        }

        /// <summary>
        /// 判断回退按钮是否能用
        /// </summary>
        private void NavBackCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_backwardNavigations.Count > 0)
                e.CanExecute = true;
        }
        /// <summary>
        /// 回退按钮执行操作
        /// </summary>
        private void NavBackCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_backwardNavigations.Count > 0)
            {
                var upFolderPath = _backwardNavigations.Pop();
                _forwardNavigations.Push(CurrentDirectory);
                CurrentDirectory = upFolderPath;
                this.file_name.Text = null;
                if (CurrentDirectory.FullPath == path)
                {
                    this.file_url.Content = CurrentDirectory.FullPath.Replace(path, "/");
                }
                else
                {
                    this.file_url.Content = CurrentDirectory.FullPath.Replace(path, "");
                }
            }
        }
        #endregion

        #region 搜索栏点击事件
        private void SearchResults(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string content = ((TextBox)sender).Text.Trim();
                if (content != "")
                {
                    IEntryModel entry;
                    entry = CurrentDirectory;
                    this.file_url.Content = "搜索结果";
                    SkyDriveProfile profile = new SkyDriveProfile(CurrentServer, sc, cts);
                    profile.ListSearchAsync(content, ListViewItems, entry);
                }
                else
                {
                    CurrentDirectory = _rootDir;
                    this.file_url.Content = "/";
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string content = this.file_name.Text.Trim();
            if (content != "")
            {
                IEntryModel entry;
                entry = CurrentDirectory;
                this.file_url.Content = "搜索结果";
                SkyDriveProfile profile = new SkyDriveProfile(CurrentServer, sc, cts);
                profile.ListSearchAsync(content, ListViewItems, CurrentDirectory);
            }
            else
            {
                CurrentDirectory = _rootDir;
                this.file_url.Content = "/";
            }
        }
        #endregion

        #region 切换到缩略图模式
        private void btnChangeView_Click(object sender, RoutedEventArgs e)
        {
            CoreManager.ViewMode mode = (CoreManager.ViewMode)Enum.Parse(typeof(CoreManager.ViewMode), CoreManager.ConfigManager.Settings.ViewMode, true);
            switch (mode)
            {
                case CoreManager.ViewMode.Details:
                    CoreManager.ConfigManager.Settings.ViewMode = CoreManager.ViewMode.Thumb.ToString(); break;
                case CoreManager.ViewMode.Thumb:
                    CoreManager.ConfigManager.Settings.ViewMode = CoreManager.ViewMode.Details.ToString(); break;
                default:
                    CoreManager.ConfigManager.Settings.ViewMode = CoreManager.ViewMode.Details.ToString(); break;
            }
            ChangeView();
        }

        private void ChangeView()
        {
            CoreManager.ViewMode mode = (CoreManager.ViewMode)Enum.Parse(typeof(CoreManager.ViewMode), CoreManager.ConfigManager.Settings.ViewMode, true);

            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(string.Format("pack://application:,,,/Assets/Images/{0}_view.png", mode.ToString().ToLower())));
            this.btnChangeView.Background = brush;
            this.btnChangeView.MyEnterBrush = brush;
            this.btnChangeView.MyMoverBrush = brush;

            this.btnChangeView.ToolTip = mode == CoreManager.ViewMode.Details ? "切换到缩略图模式" : "切换到列表模式";

            this.ThumbnailsView.IsChecked = mode != CoreManager.ViewMode.Details;
            this.DetailsView.IsChecked = mode == CoreManager.ViewMode.Details;
            this.txtViewMode.Text = mode.ToString();
            CoreManager.ConfigManager.SaveSettings();
        }
        #endregion

        #region 右键功能菜单区
        //文件列表条目双击事件
        private async void FileListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            this.file_name.Text = null;
            ListViewItem item = (ListViewItem)sender;
            var selectedItem = item.Content as SkyDriveItemModel;
            bool can = GetFolderType(selectedItem.ServerItem.ServerRelativeUrl);

            if (can)
            {
                ViewDocPage viewPage = new ViewDocPage(CurrentServer, selectedItem.ServerItem, "View");
                viewPage.Title = selectedItem.Name;
                viewPage.Show();
                viewPage.Topmost = true;
            }
            else
            {
                if (selectedItem.IsDirectory)
                {
                    _backwardNavigations.Push(CurrentDirectory);
                    CurrentDirectory = selectedItem;
                    this.file_url.Content = CurrentDirectory.FullPath.Replace(path, "");
                }
                else
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = selectedItem.Name;
                    var result = saveFileDialog.ShowDialog();
                    if (result.HasValue && result.Value)
                    {
                        string localFile = await DownloadFile(selectedItem, false);
                        System.IO.File.Copy(localFile, saveFileDialog.FileName, true);
                    }
                }
            }
        }

        //上传文件
        private void ListViewUploadFile_Click(object sender, RoutedEventArgs e)
        {
            UploadFile();
        }

        private void UploadFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog().Value)
            {
                List<TempFile> files = new List<TempFile>();
                files.Add(new TempFile(ofd.FileName));
                if (ListViewItems.Where(m => m.Name == System.IO.Path.GetFileName(ofd.FileName)).Count() > 0)
                {
                    DialogWindow dialog = new DialogWindow(() =>
                    {
                        UploadFile(files);
                    });
                    dialog.TipText = string.Format("发现同名文件，您要替换它吗？\r\n{0}\r\n", System.IO.Path.GetFileName(ofd.FileName));
                    dialog.ShowDialog();
                }
                else
                {
                    UploadFile(files);
                }
            }
        }

        private async void UploadFile(List<TempFile> files, bool? isNew = false)
        {
            var relUrl = CurrentDirectory.FullPath.Replace(_rootDir.FullPath, string.Empty).TrimStart('/');
            if (!string.IsNullOrEmpty(relUrl))
                relUrl += "/";

            FileDropHandler dropHandler = new FileDropHandler(_sm, files.ToArray(), relUrl);

            dropHandler
                       .UploadingItems
                       .Where(c => !c.IsDirectory)
                       .ToList()
                       .ForEach(c => App.CurrentApp.UploadingFileDetailPage.UploadingItems.Add(c));

            //App.CurrentApp.UploadingFileDetailPage.Show();

            await dropHandler.Handle(CurrentServer, (model, si) =>
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (si == null)
                        model.Status = "失败";
                    else
                    {
                        model.Status = "完成";
                        App.CurrentApp.UploadingFileDetailPage.AddUploadedItems(si);
                        App.CurrentApp.UploadingFileDetailPage.RemoveCompletedUploadingFile();
                    }
                }));
            });

            CurrentDirectory = CurrentDirectory;
        }

        //新建文件夹
        private void NewFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            NewFolder();
        }

        private async void NewFolder()
        {
            string folderName = "新建文件夹";
            string name = folderName;

            bool flag = true;
            int index = 1;
            do
            {
                if (this.ListViewItems
                    .Where(m => m.IsDirectory && m.Name == name).Count() == 0)
                {
                    break;
                }
                else
                {
                    name = string.Format("{0}({1})", folderName, index);
                    index++;
                }
            } while (flag);

            string createdPath = ResolveRelativePath(name);

            SkyDriveItemModel currentDirModel = CurrentDirectory as SkyDriveItemModel;
            ServerItem parentServerItem = currentDirModel.ServerItem;
            ServerItem si = await CurrentServer.CreateNewFolder(parentServerItem, createdPath.Replace("/", "\\"));

            if (si != null)
            {
                //CurrentDirectory = CurrentDirectory;
                ListViewItems.Add(new SkyDriveItemModel(_rootDir.Profile, si));

                SetPersonFileInEditMode(name, true);
            }
            else
            {
                MessageBox.Show("新建失败");
            }
        }

        private string ResolveRelativePath(string path)
        {
            string relPath = string.Empty;
            var relUrl = CurrentDirectory.FullPath.Replace(_rootDir.FullPath, string.Empty).TrimStart('/');
            if (!string.IsNullOrEmpty(relUrl))
                relPath = relUrl.TrimEnd('/') + "/" + path;
            else
                relPath = path;

            return relPath;
        }

        //设置为编辑状态
        private void SetPersonFileInEditMode(string name, bool isDirectory)
        {
            foreach (SkyDriveItemModel item in this.ListViewItems.Where(m => m.IsDirectory == isDirectory))
            {
                if (item.Name == name)
                {
                    this.FileListView.SelectedItem = item;
                    item.IsInEditMode = true;
                    break;
                }
            }
        }

        //查看缩略图或者详细信息
        private void ThumbnailsView_Click(object sender, RoutedEventArgs e)
        {
            CoreManager.ConfigManager.Settings.ViewMode = CoreManager.ViewMode.Thumb.ToString();
            ChangeView();
        }

        private void DetailsView_Click(object sender, RoutedEventArgs e)
        {
            CoreManager.ConfigManager.Settings.ViewMode = CoreManager.ViewMode.Details.ToString();
            ChangeView();
        }

        //刷新
        private void FileListCtmRefresh_Click(object sender, RoutedEventArgs e)
        {
            CurrentDirectory = CurrentDirectory;
        }

        //在线编辑和在线预览
        private void ViewDocCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            if (this.FileListView.SelectedItems.Count > 1)
                e.CanExecute = false;
        }

        private void ViewDocCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var model = this.FileListView.SelectedItem as SkyDriveItemModel;

                if (e.Parameter.Equals("ViewDoc"))
                {
                    bool can = GetFolderEditType(model.ServerItem.ServerRelativeUrl);
                    if (can)
                    {
                        ViewDocPage viewPage = new ViewDocPage(CurrentServer, model.ServerItem, "Edit");
                        viewPage.Title = model.Name;
                        viewPage.Show();
                        viewPage.Topmost = true;
                    }
                    else
                    {
                        MessageBox.Show("您所选的文件为非Office文件，无法在线编辑！");
                    }
                }
                if (e.Parameter.Equals("OnlyViewDoc"))
                {
                    bool can = GetFolderType(model.ServerItem.ServerRelativeUrl);
                    if (can)
                    {
                        ViewDocPage viewPage = new ViewDocPage(CurrentServer, model.ServerItem, "View");
                        viewPage.Title = model.Name;
                        viewPage.Show();
                        viewPage.Topmost = true;
                    }
                    else
                    {
                        MessageBox.Show("您所选的文件为非Office文件，无法在线预览！");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载文档错误.");
                Logging.Add("预览文档错误.", ex);
            }
        }

        public static bool GetFolderEditType(string filePath)
        {
            string extesion = filePath.Substring(filePath.LastIndexOf(".") + 1);
            if (extesion == "docx" || extesion == "pptx" || extesion == "xlsx")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GetFolderType(string filePath)
        {
            string extesion = filePath.Substring(filePath.LastIndexOf(".") + 1);
            if (extesion == "docx" || extesion == "pptx" || extesion == "xlsx" || extesion == "doc" || extesion == "ppt" || extesion == "xls" || extesion == "pdf")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //复制、剪切
        private const string ClipboardTextPrefix = "Easi365Disk|";
        private FileOprationType _fileOprationType = FileOprationType.Copy;

        private void CopyBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
                var selectedItem = this.FileListView.SelectedItem; ;
                if (selectedItem != null)
                {
                    SkyDriveItemModel model = selectedItem as SkyDriveItemModel;
                    e.CanExecute = !model.IsDirectory;
                }

                if (this.FileListView.SelectedItems.Count > 1)
                    e.CanExecute = false;
        }

        private void CopyBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
                SetMoveOrCopyClipboardText(FileOprationType.Copy);
        }

        private void CutBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
                SetMoveOrCopyClipboardText(FileOprationType.Move);
        }

        private void SetMoveOrCopyClipboardText(FileOprationType operType)
        {
            try
            {
                if (this.FileListView.SelectedItems.Count <= 1)
                {
                    var deptselectedItem = this.FileListView.SelectedItem;
                    SkyDriveItemModel model = deptselectedItem as SkyDriveItemModel;
                    Clipboard.SetText(ClipboardTextPrefix + model.ServerItem.Id +
                        "|" + model.ServerItem.Name + "|" + model.FullPath + "|" + model.ServerItem + "|" + model.Profile.Server.ServerRoot);
                    _fileOprationType = operType;
                }
                else
                {
                    string text = null;
                    foreach (var item in this.FileListView.SelectedItems)
                    {
                        SkyDriveItemModel model = item as SkyDriveItemModel;
                        if (!model.IsDirectory)
                        {
                            text = text + "#" + ClipboardTextPrefix + model.ServerItem.Id +
                            "|" + model.ServerItem.Name + "|" + model.FullPath + "|" + model.ServerItem + "|" + model.Profile.Server.ServerRoot;
                        }
                    }
                    Clipboard.SetText(text.Substring(1));
                    _fileOprationType = operType;
                }
            }
            catch { }
        }

        //粘贴
        private void PasteBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                var clipText = Clipboard.GetText();
                if (clipText.StartsWith(ClipboardTextPrefix))
                {
                    e.CanExecute = true;
                }
            }
        }

        private void PasteBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            {
                //是否覆盖文件
                bool overwrite = false;

                //取出剪切板中的数据
                var clipText = Clipboard.GetText();
                var fileInfoText = clipText.Split(new char[] { '#' });
                foreach (var t in fileInfoText)
                {
                    var paramText = t.Substring(ClipboardTextPrefix.Length);
                    var id = Convert.ToInt32(paramText.Split(new char[] { '|' })[0]);
                    var name = paramText.Split(new char[] { '|' })[1];
                    var url = paramText.Split(new char[] { '|' })[2];
                    var item = paramText.Split(new char[] { '|' })[3];
                    var rootUrl = paramText.Split(new char[] { '|' })[4];

                    //判断当前列表中是否有已经存在的文件
                    var fileExists = ListViewItems.Any(f => f.Name == name);

                    if (fileExists)
                    {
                        DialogWindow dialogWindow = new DialogWindow();
                        dialogWindow.TipText = name + " 已经存在，是否覆盖已经存在的文件！";
                        var dlgResut = dialogWindow.ShowDialog();
                        if (!dlgResut.Value)
                            return;
                        overwrite = dlgResut.Value;
                    }

                    //处理文件路径
                    string relPath = string.Empty;
                    var relUrl = CurrentDirectory.FullPath.Replace(_rootDir.FullPath, string.Empty).TrimStart('/');
                    if (!string.IsNullOrEmpty(relUrl))
                        relPath = relUrl.TrimEnd('/') + "/" + name;
                    else
                        relPath = name;
                    string serverRelPath = CurrentServer.RootFolderServerRelativeUrl + "/" + relPath.Replace('\\', '/');

                    //处理文件
                    if (_fileOprationType == FileOprationType.Copy)
                    {
                        CurrentServer.FilePaste(url, serverRelPath, overwrite);
                    }

                    if (_fileOprationType == FileOprationType.Move)
                    {
                        CurrentServer.FilePaste(url, serverRelPath, overwrite);
                        //_sm.DeptServer.DeleteMoveFile(url);
                    }
                }

                System.Threading.Thread.Sleep(300);
                CurrentDirectory = CurrentDirectory;
            }
        }

        //删除
        private void DelBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
                    bool canExecute = this.FileListView.SelectedItem != null;
                    e.CanExecute = canExecute;
        }

        private void DeleteRemoteFile(ListView listView, IEntryModel currentDir, Action callback = null)
        {
            //删除多个文件
            if (listView.SelectedItems.Count > 1)
            {
                DialogWindow dialog = new DialogWindow(async () =>
                {
                    this.fileListOperation.LoadingText = "正在删除...";
                    this.fileListOperation.Visibility = System.Windows.Visibility.Visible;

                    foreach (var item in listView.SelectedItems)
                    {
                        var delItem = item as SkyDriveItemModel;
                        //这里是直接删除文件 需要添加异步操作.capad
                        if (delItem != null)
                        {
                            if (delItem.IsDirectory)
                                await currentDir.Profile.Server.DeleteFolder(delItem.ServerItem, delItem.FullPath);
                            else
                                await currentDir.Profile.Server.DeleteFile(delItem.ServerItem, delItem.FullPath);
                        }
                    }

                    this.fileListOperation.Visibility = System.Windows.Visibility.Collapsed;

                    if (callback != null)
                        callback();
                });
                dialog.TipText = "确定要删除选中的 " + listView.SelectedItems.Count + " 个文件吗？";
                //dialog.Owner = this;
                dialog.ShowDialog();
            }
            else
            {
                //删除单个文件
                var selectedItem = listView.SelectedItem as SkyDriveItemModel;

                if (selectedItem != null)
                {
                    DialogWindow dialog = new DialogWindow(async () =>
                    {
                        this.fileListOperation.LoadingText = "正在删除...";
                        this.fileListOperation.Visibility = System.Windows.Visibility.Visible;

                        //这里是直接删除文件 需要添加异步操作.capad
                        if (selectedItem.IsDirectory)
                            await currentDir.Profile.Server.DeleteFolder(selectedItem.ServerItem, selectedItem.FullPath);
                        else
                            await currentDir.Profile.Server.DeleteFile(selectedItem.ServerItem, selectedItem.FullPath);

                        //取消loading动画
                        this.fileListOperation.Visibility = System.Windows.Visibility.Collapsed;

                        if (callback != null)
                            callback();
                    });
                    dialog.TipText = "确定要删除 " + selectedItem.Name + " 吗？";
                    //dialog.Owner = this;
                    dialog.ShowDialog();
                }
            }
        }

        //删除文件
        private void DelBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
                    DeleteRemoteFile(this.FileListView, CurrentDirectory,
                        () =>
                        {
                            //刷新列表
                            CurrentDirectory = CurrentDirectory;
                        });
            
        }

        //下载
        private void DownloadFileBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var listViewItem = e.OriginalSource as ListViewItem;
            // SkyDriveItemModel selectedItem = this.FileListView.SelectedItem as SkyDriveItemModel;
            if (listViewItem != null)
            {
                var model = listViewItem.Content as SkyDriveItemModel;

                if (model != null && model.IsDirectory)
                    e.CanExecute = false;
                else
                    e.CanExecute = true;
            }

            SetRoutedCommandUnuse(e);
        }

        // 取消下载能否执行
        private void CancelDownloadingBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // 取消下载操作执行
        private void CancelDownloadingBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Button cancelBtn = (Button)e.OriginalSource;
            DownloadState state = cancelBtn.Tag as DownloadState;

            //取消下载
            if (state != null)
            {
                state.CancellationToken.Cancel(true);
                state.Reset();
            }
        }

        //多选文件时 将右键菜单不可用项目置灰
        private void SetRoutedCommandUnuse(CanExecuteRoutedEventArgs e)
        {
            if (this.FileListView.SelectedItems.Count > 1)
                e.CanExecute = true;
        }

        private async void DownloadFileBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //var originalSource = e.OriginalSource as SkyDriveItemModel;


            //FollowViewDownload 关注视图的文件下载
            //PersonalViewDownload 个人视图的文件下载

            var commandParameter = Convert.ToString(e.Parameter);

            switch (commandParameter)
            {
                case "FollowViewDownload":
                    {
                        try
                        {
                            ListViewItem item = (ListViewItem)e.OriginalSource;
                            var model = item.Content as SkyDriveItemModel;

                            PersonalServerSide personalServer = CurrentServer as PersonalServerSide;

                            SaveFileDialog saveFileDialog = new SaveFileDialog();
                            saveFileDialog.FileName = model.Name;
                            var result = saveFileDialog.ShowDialog();
                            if (result.HasValue && result.Value)
                            {
                                if (model.DownloadState == null)
                                    model.DownloadState = new DownloadState();

                                await personalServer.DownLoadFile(model.FullPath, saveFileDialog.FileName, model.DownloadState);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("保存文件错误.");
                            Logging.Add("保存收藏文件错误.", ex);
                        }
                    }
                    break;
                case "PersonalViewDownload":
                    {
                        ListViewItem item = (ListViewItem)e.OriginalSource;
                        var selectedItem = item.Content as SkyDriveItemModel;

                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = selectedItem.Name;
                        var result = saveFileDialog.ShowDialog();
                        if (result.HasValue && result.Value)
                        {
                            string localFile = await DownloadFile(selectedItem, false);
                            System.IO.File.Copy(localFile, saveFileDialog.FileName, true);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private async Task<string> DownloadFile(SkyDriveItemModel selectedItem, bool openFile)
        {
            string localpath = string.Empty;

            if (selectedItem != null)
            {
                {
                    try
                    {
                        //如果当前有下载任务 则直接返回
                        if (this.ListViewItems.Any(entry => entry.DownloadState.IsDownloading))
                            localpath = string.Empty;

                        bool needDownload = CurrentServer.CheckIfFileNeedDownload(selectedItem.ServerItem,
                            () =>
                            {
                                DialogWindow dialogWindow = new DialogWindow();
                                dialogWindow.TipText = "服务器的文件版本比较新，是否下载文件？";
                                var dlgResut = dialogWindow.ShowDialog();

                                return dlgResut.Value;
                            });
                        if (needDownload)
                            selectedItem.DownloadState.IsDownloading = true;//更改下载状态的显示或隐藏

                        localpath = await CurrentServer.DownloadFile(
                           selectedItem.ServerItem,
                           needDownload,
                           selectedItem.DownloadState);

                        // 如果取消下载 则直接返回
                        if (selectedItem.DownloadState.CancellationToken.IsCancellationRequested)
                            return "";

                        if (needDownload)
                            selectedItem.DownloadState.IsDownloading = false;

                        // 下载完成后是否需要打开文件
                        if (openFile)
                        {
                            if (!string.IsNullOrEmpty(localpath))
                            {
                                //监控文件
                                CoreManager.FileWatcher.SetFileMonitor(localpath);

                                System.Diagnostics.Process p = new System.Diagnostics.Process();
                                p.StartInfo.FileName = localpath;
                                p.Start();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Add("下载文件出错.", ex);
                    }
                }
            }

            return localpath;
        }

        //重命名
        private void PersonRenameBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
                SkyDriveItemModel selectedItem = this.FileListView.SelectedItem as SkyDriveItemModel;
                if (selectedItem != null)
                {
                    e.CanExecute = true;
                }
                SetRoutedCommandUnuse(e);
        }

        private void PersonRenameBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
                SkyDriveItemModel selectedItem = this.FileListView.SelectedItem as SkyDriveItemModel;
                if (selectedItem != null)
                {
                    selectedItem.IsInEditMode = true;
                }
        }

        public void PersonFileRename_LostFocus(EditableTextBlock textBox)
        {
            try
            {
                if (this.ListViewItems
                    .Where(m =>
                    {
                        var item = m as SkyDriveItemModel;
                        return item.IsDirectory == textBox.IsDirectory &&
                            item.Name == textBox.Text &&
                            item.ServerItem.Id != textBox.Id;
                    }).Count() == 0)
                {
                    CurrentServer.ModifyFileName(textBox.Id, textBox.Text);
                }
                else
                {
                    DialogWindow dialog = new DialogWindow(null, DialogWindow.DialogType.Alert);
                    dialog.TipText = "文件已存在";
                    dialog.ShowDialog();
                }
                CurrentDirectory = CurrentDirectory;
            }
            catch (Exception ex)
            {
                CurrentDirectory = CurrentDirectory;
                Logging.Add("重命名失败", ex);
            }
        }

        //拖拽上传
        private void FileListView_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private async void FileListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                List<TempFile> files = new List<TempFile>();
                string[] dropfiles = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in dropfiles)
                {
                    if (ListViewItems.Where(m => m.Name == System.IO.Path.GetFileName(file)).Count() > 0)
                    {
                        DialogWindow dialog = new DialogWindow(() =>
                        {
                            files.Add(new TempFile(file));
                        });
                        dialog.TipText = string.Format("发现同名文件，您要替换它吗？\r\n{0}\r\n", System.IO.Path.GetFileName(file));
                        dialog.ShowDialog();
                    }
                    else
                    {
                        files.Add(new TempFile(file));
                    }
                }

                if (files.Count > 0)
                {
                    var relUrl = CurrentDirectory.FullPath.Replace(_rootDir.FullPath, string.Empty).TrimStart('/');
                    if (!string.IsNullOrEmpty(relUrl))
                        relUrl += "/";

                    FileDropHandler dropHandler = new FileDropHandler(_sm, files.ToArray(), relUrl);

                    dropHandler
                        .UploadingItems
                        .Where(c => !c.IsDirectory)
                        .ToList()
                        .ForEach(c => App.CurrentApp.UploadingFileDetailPage.UploadingItems.Add(c));

                    //App.CurrentApp.UploadingFileDetailPage.Show();

                    await dropHandler.Handle(CurrentServer, (model, si) =>
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            if (si == null)
                                model.Status = "失败";
                            else
                            {
                                model.Status = "完成";
                                App.CurrentApp.UploadingFileDetailPage.AddUploadedItems(si);
                                App.CurrentApp.UploadingFileDetailPage.RemoveCompletedUploadingFile();
                            }
                            //if (!model.IsDirectory)
                            //    ListViewItems.Add(new SkyDriveItemModel(_rootDir.Profile, si));

                        }));
                    });

                    CurrentDirectory = CurrentDirectory;

                }
            }
        }

        //清楚ListView选中项
        private void PersonFileListView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.FileListView.SelectedItem = null;
            foreach (SkyDriveItemModel file in this.FileListView.Items)
            {
                file.IsInEditMode = false;
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Bread
        /// <summary>
        /// Convert the path from visual to logical or vice versa:
        /// </summary>
        private void BreadcrumbBar_PathConversion(object sender, PathConversionEventArgs e)
        {
            if (e.Mode == PathConversionEventArgs.ConversionMode.DisplayToEdit)
            {
                if (e.DisplayPath.StartsWith(@"Computer\", StringComparison.OrdinalIgnoreCase))
                {
                    e.EditPath = e.DisplayPath.Remove(0, 9);
                }
                else if (e.DisplayPath.StartsWith(@"Network\", StringComparison.OrdinalIgnoreCase))
                {
                    string editPath = e.DisplayPath.Remove(0, 8);
                    editPath = @"\\" + editPath.Replace('\\', '/');
                    e.EditPath = editPath;
                }
            }
            else
            {
                if (e.EditPath.StartsWith("c:", StringComparison.OrdinalIgnoreCase))
                {
                    e.DisplayPath = @"Desktop\Computer\" + e.EditPath;
                }
                else if (e.EditPath.StartsWith(@"/"))
                {
                    e.DisplayPath = @"Desktop\Network\" + e.EditPath.Remove(0, 2).Replace('/', '\\');
                }
            }
        }

        private void BreadcrumbBar_PopulateItems(object sender, Odyssey.Controls.BreadcrumbItemEventArgs e)
        {
            BreadcrumbItem item = e.Item;
            if (item.Items.Count == 0)
            {
                PopulateFolders(item);
                e.Handled = true;
            }
        }

        private static void PopulateFolders(BreadcrumbItem item)
        {
            List<FolderItem> folderItems = GetFolderItemsFromBreadcrumb(item);
            item.ItemsSource = folderItems;
        }

        private void bar_BreadcrumbItemDropDownOpened(object sender, BreadcrumbItemEventArgs e)
        {
            BreadcrumbItem item = e.Item;

            // only repopulate, if the BreadcrumbItem is dynamically generated which means, item.Data is a  pointer to itself:
            if (!(item.Data is BreadcrumbItem))
            {
                UpdateFolderItems(item);
            }
        }

        private void UpdateFolderItems(BreadcrumbItem item)
        {
            List<FolderItem> actualFolders = GetFolderItemsFromBreadcrumb(item);
            List<FolderItem> currentFolders = item.ItemsSource as List<FolderItem>;
            currentFolders.Clear();
            currentFolders.AddRange(actualFolders);

        }

        private static List<FolderItem> GetFolderItemsFromBreadcrumb(BreadcrumbItem item)
        {
            BreadcrumbBar bar = item.BreadcrumbBar;
            string path = bar.PathFromBreadcrumbItem(item);
            string trace = item.TraceValue;
            string[] subFolders;
            if (trace.Equals("Computer"))
            {
                subFolders = GetDrives(bar.SeparatorString).ToArray();
            }
            else
            {
                try
                {
                    subFolders = (from dir in System.IO.Directory.GetDirectories(path + "\\") select System.IO.Path.GetFileName(dir)).ToArray();
                }
                catch
                {
                    //maybe we don't have access!
                    subFolders = new string[] { };
                }
            }
            List<FolderItem> folderItems = (from folder in subFolders
                                            orderby folder
                                            select new FolderItem { Folder = folder })
                                            .ToList();
            return folderItems;
        }

        private static IEnumerable<string> GetDrives(string separatorString)
        {
            int separatorLength = separatorString.Length;
            var folders = from drive in System.IO.Directory.GetLogicalDrives()
                          select drive.EndsWith(separatorString) ? drive.Remove(drive.Length - separatorLength) : drive;
            return folders.AsEnumerable();
        }
        #endregion

        #region 绑定左侧导航栏
        public async void BindDocList()
        {
            var doc = await GetDocList();

            ListBox l = new ListBox();
            l.ItemsSource = doc;
            l.ItemTemplate = Resources["ExpanderListItemTemplete"] as DataTemplate;
            l.Style = Resources["ExpanderListBox"] as Style;
            l.ItemContainerStyle = Resources["ExpanderListItemStyle"] as Style;
            l.MouseLeftButtonUp += l_MouseLeftButtonUp;
            FileDocList.Children.Add(l);
        }

        private async void l_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SkyDriveProfile profile = new SkyDriveProfile(CurrentServer, sc, cts);

            var selectBox = (ListBox)(sender);
            var selectItem = selectBox.SelectedItem as Spaces;
            if (selectItem != null)
            {
                this.DeptLoading.Visibility = System.Windows.Visibility.Visible;
                await Task<bool>.Factory.StartNew(delegate()
                {
                    try
                    {
                        CurrentServer.RootFolderServerRelativeUrl = selectItem.SpaceUri;
                        CurrentServer.RemoteLibrary = CurrentServer.ClientCtx.Web.Lists.GetByTitle(selectItem.SpaceTitle);

                        _rootDir = SkyDriveItemModel.DefaultSkyDriveItemModel(profile,selectItem.SpaceUri);
                        CurrentDirectory = _rootDir;

                        _backwardNavigations.Clear();
                        _forwardNavigations.Clear();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                });
                this.DeptLoading.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public Task<ObservableCollection<Spaces>> GetDocList()
        {
            return Task<ObservableCollection<Spaces>>.Factory.StartNew(() =>
            {
                ObservableCollection<Spaces> DocList = new ObservableCollection<Spaces>();
                try
                {
                    foreach (var list in CurrentServer.RemoteLibrarys)
                    {
                        DocList.Add(new Spaces { SpaceTitle = list.Title, SpaceUri = list.RootFolder.ServerRelativeUrl });
                    }
                }
                catch (Exception ex)
                {
                    Logging.Add("加载文档报错", ex);
                }
                return DocList;
            });
        }
        #endregion

        #region 标签按钮的点击事件
        

        //个人空间
        private void PS_Click(object sender, RoutedEventArgs e)
        {
            space = "ps";
            CurrentServer = _sm.PersonalServer;
            Space = space;
            this.FileDocList.Children.Clear();
            BindDocList();
            CurrentDirectory = _rootDir;
        }

        //部门空间
        private void DS_Click(object sender, RoutedEventArgs e)
        {
            space = "ds";
            CurrentServer = _sm.DepartmentServer;
            Space = space;
            this.FileDocList.Children.Clear();
            BindDocList();
            CurrentDirectory = _rootDir;
        }

        //公司空间
        private void CS_Click(object sender, RoutedEventArgs e)
        {
            space = "cs";
            CurrentServer = _sm.CompanyServer;
            Space = space;
            this.FileDocList.Children.Clear();
            BindDocList();
            CurrentDirectory = _rootDir;
        }
        #endregion
    }

    internal enum FileOprationType
    {
        Copy,
        Move
    }
}
