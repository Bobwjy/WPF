using ClientLib;
using ClientLib.Core;
using ClientLib.Entities;
using ClientLib.Services;
using ClientLib.Utilities;
using Easi365UI.Models;
using Easi365UI.Models.SkyDrive;
using Easi365UI.Windows.Controls;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Easi365UI
{
    /// <summary>
    /// SendFileWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SendFileWindow : EasiWindow
    {
        public SendFileWindow()
        {
            InitializeComponent();

            //点击窗体头部区域拖拽整个窗体移动
            this.ContentHeader.MouseLeftButtonDown += HeaderGrid_MouseLeftButtonDown;
        }

        void HeaderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public SendFileWindow(SyncManager sm, SkyDriveItemModel skyDriveItemModel)
            : this()
        {
            _sm = sm;
            _skyDriveItemModel = skyDriveItemModel;
        }


        SkyDriveItemModel _skyDriveItemModel;
        SyncManager _sm = null;
        CreateWebService _createWebService;
        TreeViewModel firstLevelItem = null;
        ObservableCollection<TreeViewModel> spaceList;
        string folderIcoUrl = "/assets/ui/small/folder.png";

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            spaceList = await BindDataToTree();
            if (spaceList != null) {
                this.SpacesTreeView.ItemsSource = spaceList;
                this.ucSpaceListLoading.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private async Task<string> DownLoadFile()
        {
            bool needDownload = _sm.PersonalServer.CheckIfFileNeedDownload(_skyDriveItemModel.ServerItem,
                () =>
                {
                    DialogWindow dialogWindow = new DialogWindow();
                    dialogWindow.TipText = "服务器的文件版本比较新，是否下载文件？";
                    var dlgResut = dialogWindow.ShowDialog();

                    return dlgResut.Value;
                });

            if (needDownload)
                _skyDriveItemModel.DownloadState.IsDownloading = true;//更改下载状态的显示或隐藏

           string localpath = await _sm.PersonalServer.DownloadFile(
               _skyDriveItemModel.ServerItem,
               needDownload,
               _skyDriveItemModel.DownloadState);

            if (needDownload)
                _skyDriveItemModel.DownloadState.IsDownloading = false;

            return localpath;
        }

        private async Task SendFile(ServerSide server,string localFilePath,string remotePath)
        {
            if (string.IsNullOrEmpty(localFilePath))
                throw new ArgumentException("localFilePath");

            UploadFileModel uploadModel = new UploadFileModel(new ClientItem(new System.IO.FileInfo(localFilePath)));
            await server.CreateNewFileAsync(uploadModel.ClientItem,
                       null,
                       uploadModel.UploadState,
                       remotePath,
                       null);
        }

        private System.Collections.Generic.List<ServerItem> _rootFolderItems;

        private async void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            this.ucSpaceListLoading.Visibility = System.Windows.Visibility.Visible;
            TreeViewItem expandedItem = (TreeViewItem)e.OriginalSource;
            expandedItem.IsSelected = true;
            var selectedItem = this.SpacesTreeView.SelectedItem as TreeViewModel;
            selectedItem.Children.Remove(TreeViewModel._temp);

            if (selectedItem.Children.Count == 0)
            {
                string folderUrl = "";
                ServerSide serverSide = null;
                ObservableCollection<TreeViewModel> tvModeList = new ObservableCollection<TreeViewModel>();

                if (selectedItem.TopLevel == CoreManager.SpaceCategory.Company)
                {
                    serverSide = _sm.CompanyServerSide as ServerSide;
                    var ctx = serverSide.ClientCtx;
                    ctx.Load(ctx.Web.Lists);
                    ctx.ExecuteQuery();
                    if (selectedItem.NodeName == "公司空间")
                    {
                        foreach (var list in serverSide.RemoteLibrarys) {
                            var folderRelativeUrl = list.RootFolder.ServerRelativeUrl;
                            folderIcoUrl = "/assets/images/document.png";
                            TreeViewModel tvm = new TreeViewModel(list.Title, selectedItem.TopLevel, selectedItem.SpaceUri, true, true, folderRelativeUrl, folderIcoUrl, serverSide,list);
                            selectedItem.Children.Add(tvm);
                            tvModeList.Add(tvm);
                        }
                    }
                    else {
                        serverSide.RemoteLibrary = selectedItem.RemoteList;
                        folderUrl = selectedItem.FolderRelativeUrl;
                        _rootFolderItems = await serverSide.GetFolderInFolder(folderUrl, 0);
                        foreach (var item in _rootFolderItems)
                        {
                            var folderRelativeUrl = folderUrl + "/" + item.Name;
                            folderIcoUrl = "/assets/ui/small/folder.png";
                            TreeViewModel tvm = new TreeViewModel(item.Name, selectedItem.TopLevel, selectedItem.SpaceUri, true, true, folderRelativeUrl, folderIcoUrl, serverSide,selectedItem.RemoteList);
                            selectedItem.Children.Add(tvm);
                            tvModeList.Add(tvm);
                        }
                    }
                }
                else if (selectedItem.TopLevel == CoreManager.SpaceCategory.SubWebSpace) {
                    if (selectedItem.NodeName == "其他空间")
                    {
                        serverSide = _sm.CompanyServerSide as ServerSide;
                        var ctx = this._sm.CompanyServerSide.ClientCtx;
                        ctx.Load(ctx.Web.Webs);
                        ctx.ExecuteQuery();
                        foreach (var subWeb in ctx.Web.Webs)
                        {
                            if (subWeb.ServerRelativeUrl.IndexOf("/DeptSpace") > -1 || subWeb.ServerRelativeUrl.IndexOf("/CooSpace") > -1) continue;
                            var userWebPerm = subWeb.GetUserEffectivePermissions(ctx.Web.CurrentUser.LoginName);
                            ctx.ExecuteQuery();
                            BasePermissions baseWebPerm = userWebPerm.Value;
                            if (baseWebPerm.Has(PermissionKind.Open))
                            {
                                var webUrl = subWeb.Url;
                                folderIcoUrl = "/assets/images/subweb.png";
                                TreeViewModel tvm = new TreeViewModel(subWeb.Title, selectedItem.TopLevel, selectedItem.SpaceUri, false, true, webUrl, folderIcoUrl, null);
                                selectedItem.Children.Add(tvm);
                                tvModeList.Add(tvm);
                            }
                        }
                    }
                    else {
                        if (!selectedItem.IsFolder)
                        {
                            serverSide = _sm.SubWebServerManager.FindSubWebSpace(selectedItem.FolderRelativeUrl);
                            var ctx = serverSide.ClientCtx;
                            ctx.Load(ctx.Web.Lists);
                            ctx.ExecuteQuery();
                            foreach (var list in ctx.Web.Lists)
                            {
                                var userListPerm = list.GetUserEffectivePermissions(ctx.Web.CurrentUser.LoginName);
                                ctx.ExecuteQuery();
                                BasePermissions baseListPerm = userListPerm.Value;
                                if (baseListPerm.Has(PermissionKind.ViewListItems))
                                {
                                    ctx.Load(list,
                                    lib => lib.RootFolder,
                                    lib => lib.IsSiteAssetsLibrary);
                                    ctx.Load(list.RootFolder, folder => folder.ServerRelativeUrl);
                                    ctx.ExecuteQuery();

                                    if (list.BaseType.ToString() == "DocumentLibrary" && !list.IsApplicationList && list.Hidden == false)
                                    {
                                        var folderRelativeUrl = list.RootFolder.ServerRelativeUrl;
                                        folderIcoUrl = "/assets/images/document.png";
                                        TreeViewModel tvm = new TreeViewModel(list.Title, selectedItem.TopLevel, selectedItem.SpaceUri, true, true, folderRelativeUrl, folderIcoUrl, serverSide);
                                        selectedItem.Children.Add(tvm);
                                        tvModeList.Add(tvm);
                                    }
                                }
                            }
                        }
                        else
                        {
                            serverSide = selectedItem.ServerSide;
                            if (selectedItem.IsLoadChildFolder) folderUrl = selectedItem.FolderRelativeUrl;
                            if (serverSide.RemoteLibrary == null)
                            {
                                serverSide.RemoteLibrary = serverSide.ClientCtx.Web.Lists.GetByTitle(selectedItem.NodeName);
                                serverSide.ClientCtx.ExecuteQuery();

                                serverSide.ClientCtx.Load(serverSide.RemoteLibrary,
                                    lib => lib.Id,
                                    lib => lib.RootFolder);
                                serverSide.ClientCtx.Load(serverSide.RemoteLibrary.RootFolder, folder => folder.ServerRelativeUrl);
                                serverSide.ClientCtx.ExecuteQuery();
                                serverSide.RootFolderServerRelativeUrl = serverSide.RemoteLibrary.RootFolder.ServerRelativeUrl;
                            }

                            _rootFolderItems = await serverSide.GetFolderInFolder(folderUrl, 0);
                            foreach (var item in _rootFolderItems)
                            {
                                var folderRelativeUrl = folderUrl + "/" + item.Name;
                                folderIcoUrl = "/assets/ui/small/folder.png";
                                TreeViewModel tvm = new TreeViewModel(item.Name, selectedItem.TopLevel, selectedItem.SpaceUri, true, true, folderRelativeUrl, folderIcoUrl, serverSide);
                                selectedItem.Children.Add(tvm);
                                tvModeList.Add(tvm);
                            }
                        }
                    }
                }
                else
                {
                    if (selectedItem.TopLevel == CoreManager.SpaceCategory.CooSpace)
                        serverSide = _sm.CollaborationServerManager.FindCollaborationSpace(selectedItem.SpaceUri, CoreManager.SpaceCategory.CooSpace) as CollaborationServerSide;
                    else
                        serverSide = _sm.CollaborationServerManager.FindCollaborationSpace(selectedItem.SpaceUri, CoreManager.SpaceCategory.DeptSpace) as CollaborationServerSide;

                    folderUrl = serverSide.RemoteLibrary.RootFolder.ServerRelativeUrl;
                    if (selectedItem.IsLoadChildFolder) folderUrl = selectedItem.FolderRelativeUrl;

                    _rootFolderItems = await serverSide.GetFolderInFolder(folderUrl, 0);
                    foreach (var item in _rootFolderItems)
                    {
                        var folderRelativeUrl = folderUrl + "/" + item.Name;
                        folderIcoUrl = "/assets/ui/small/folder.png";
                        TreeViewModel tvm = new TreeViewModel(item.Name, selectedItem.TopLevel, selectedItem.SpaceUri, true, true, folderRelativeUrl, folderIcoUrl, serverSide);
                        selectedItem.Children.Add(tvm);
                        tvModeList.Add(tvm);
                    }
                }
                expandedItem.ItemsSource = tvModeList;
            }
            this.ucSpaceListLoading.Visibility = System.Windows.Visibility.Collapsed;
        }

        public async Task<ObservableCollection<TreeViewModel>> BindDataToTree()
        {
            ObservableCollection<TreeViewModel> treeView = new ObservableCollection<TreeViewModel>();
            try
            {
                firstLevelItem = new TreeViewModel("所有空间");
                //treeView.Add(firstLevelItem);

                TreeViewModel tvSpace = new TreeViewModel("公司空间", CoreManager.SpaceCategory.Company, "");
                TreeViewModel tvCooSpace = new TreeViewModel("协作空间", CoreManager.SpaceCategory.CooSpace);
                TreeViewModel tvDeptSpace = new TreeViewModel("部门空间", CoreManager.SpaceCategory.DeptSpace);
                TreeViewModel tvSubWebSpace = new TreeViewModel("其他空间", CoreManager.SpaceCategory.SubWebSpace);

                firstLevelItem.Children.Add(tvSpace);
                firstLevelItem.Children.Add(tvCooSpace);
                firstLevelItem.Children.Add(tvDeptSpace);
                firstLevelItem.Children.Add(tvSubWebSpace);

                treeView.Add(tvSpace);
                treeView.Add(tvCooSpace);
                treeView.Add(tvDeptSpace);
                treeView.Add(tvSubWebSpace);

                var cooSpaceListRoot = string.Format("{0}/{1}", CoreManager.ConfigManager.Settings.ServerUrl, Constants.SpaceSite.CooSpace);
                ClientContext cooSpaceContent = null;
                cooSpaceContent = new ClientContext(cooSpaceListRoot);
                cooSpaceContent.Credentials = App.spCredentials;
                _createWebService = new CreateWebService(cooSpaceContent);
                var cooSpacesList = await _createWebService.GetSpacesList(Constants.SpaceSite.CooSpaceListTitle);
                
                foreach (var space in cooSpacesList)
                {
                    if (space.IsCreated)
                    {
                        var webRelativeUrl = string.Format("/{0}/{1}", Constants.SpaceSite.CooSpace,space.SpaceUri);
                        tvCooSpace.Children.Add(new TreeViewModel(space.SpaceTitle, CoreManager.SpaceCategory.CooSpace, space.SpaceUri, false, false, webRelativeUrl));
                    }
                }

                _createWebService = new CreateWebService(_sm.CompanyServerSide.ClientCtx);
                var deptSpacesList = await _createWebService.GetSpacesList(Constants.SpaceSite.DeptSpaceListTitle);
                foreach (var space in deptSpacesList)
                {
                    var webRelativeUrl = string.Format("/{0}/{1}", Constants.SpaceSite.Depeartment, space.SpaceUri);
                    tvDeptSpace.Children.Add(new TreeViewModel(space.SpaceTitle, CoreManager.SpaceCategory.DeptSpace, space.SpaceUri, false, false, webRelativeUrl));
                }

                return treeView;
            }
            catch (Exception ex)
            {
                Logging.Add("发送文件窗体:加载空间列表报错",ex);
                return null;
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SendFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var checkedItems = TreeViewModel.GetCheckedItems(firstLevelItem);
            string localPath = await DownLoadFile();
            int waitingCount = 1;

            this.ucSpaceListLoading.Visibility = System.Windows.Visibility.Visible;
            this.ucSpaceListLoading.LoadingText = "正在发送 " + waitingCount + "/" + checkedItems.Count;
            foreach (var item in checkedItems)
            {
                this.ucSpaceListLoading.LoadingText = "正在发送 " + waitingCount + "/" + checkedItems.Count;

                string relativePath = item.FolderRelativeUrl.Substring(item.ServerSide.RootFolderServerRelativeUrl.Length);

                await SendFile(item.ServerSide, localPath,
                    item.FolderRelativeUrl.Substring(item.ServerSide.RootFolderServerRelativeUrl.Length) + '/' + _skyDriveItemModel.Name);

                waitingCount += 1;
            }
            this.Close();
        }
    }
}
