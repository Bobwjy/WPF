using Easi365UI.Models;
using Easi365UI.Windows.Controls;
﻿using ClientLib;
using ClientLib.Entities;
using Easi365UI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Collections.ObjectModel;

namespace Easi365UI
{
    /// <summary>
    /// ShareWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShareWindow : EasiWindow
    {
        ServerSide _server;
        ServerItem _si;
        Action _callback;
        List<SharedUserModel> _sharedUsersCache = new List<SharedUserModel>();


        public ObservableCollection<SharedUserModel> SharedUsers { get; set; }
        public ObservableCollection<CheckedUser> CheckedUsers { get; set; }
        public ObjectDataProvider provider { get; set; }
        public List<OrgViewModel> firstLevelItems { get; set; }
        public ICollectionView view { get; set; }
        public OrgViewModel rootItem { get; set; }

        public ShareWindow()
        {
            InitializeComponent();
            this.Loaded += ShareWindow_Loaded;

            this.SharedUsers = new ObservableCollection<SharedUserModel>();
            this.DataContext = this;

            this.CheckedUsers = new ObservableCollection<CheckedUser>();

            provider = FindResource("ShareProvider") as ObjectDataProvider;
            firstLevelItems = provider.Data as List<OrgViewModel>;
            view = CollectionViewSource.GetDefaultView(firstLevelItems);
            rootItem = view.CurrentItem as OrgViewModel;
            if(rootItem != null) rootItem.CheckedItems.Clear();


            //点击窗体头部区域拖拽整个窗体移动
            this.ContentHeader.MouseLeftButtonDown += HeaderGrid_MouseLeftButtonDown;
        }

        public ShareWindow(ServerSide server,ServerItem si,Action callback = null)
            : this()
        {
            _server = server;
            _si = si;
            _callback = callback;
        }

        void HeaderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        async void ShareWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var users = await _server.GetSharingInformation(_si);
            foreach (SharedUserModel usr in users)
                SharedUsers.Add(usr);

            //已经共享的用户副本
            foreach (SharedUserModel usr in SharedUsers)
                _sharedUsersCache.Add(new SharedUserModel() 
                { 
                    UserName = usr.UserName,
                    UserRole = usr.UserRole,
                    LoginName = usr.LoginName
                });

            this.txtbFileUrl.Text = _server.ClientCtx.Url + _si.ServerRelativeUrl;
        }

        //最小化登录窗体
        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        //关闭登录窗体
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TreeItemChk_Click(object sender, RoutedEventArgs e)
        {
            CheckedUsers.Clear();
            foreach (OrgViewModel checkItem in rootItem.CheckedItems)
            {
                OrgViewModel dv = ((OrgViewModel)(checkItem));
                if (dv.IsUser)
                {
                    CheckedUser cu = new CheckedUser();
                    cu.Id = dv.UserId;
                    cu.UserName = dv.DisplayText;
                    cu.Account = dv.Account;
                    CheckedUsers.Add(cu);
                }
            }

            this.CheckedUsersLst.ItemsSource = CheckedUsers;
        }

        private void btnRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            var user = this.CheckedUsers.FirstOrDefault(m => m.Id == id);
            if (user != null)
            {
                this.CheckedUsers.Remove(user);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void Share_Click(object sender, RoutedEventArgs e)
        {
            //if (CheckedUsers.Count == 0)
            //    MessageBox.Show("请选择需要添加权限的用户");

            //当前分配给用户的权限
            UserRole role = UserRole.View;
            if (cbRole.SelectedIndex == 1)
                role = UserRole.Edit;
            //CheckedUsers
            var users = this
                .CheckedUsers
                .Select(u => u.Account)
                .ToArray();

            this.shareLoading.Visibility = System.Windows.Visibility.Visible;
            //给当前选定的人员分配共享权限
            if(users.Length > 0)
                await _server.ShareFileOrFolder(_si, users, role);

            //已经享有的权限变更
            var diffSharedUsers = SharedUsers
                .Except(_sharedUsersCache, new SharedUserEquality());
            var diffSharedUsersGroup = diffSharedUsers
                .GroupBy(s => s.UserRole);

            foreach (var g in diffSharedUsersGroup)
            {
                var userNames = g
                    .Select(s => s.LoginName)
                    .ToArray();

                await _server.ShareFileOrFolder(_si, userNames, g.Key,true);
            }

            this.shareLoading.Visibility = System.Windows.Visibility.Collapsed;

            if (_callback != null)
                _callback();

            this.Close();
        }

        private void LinkCopyFileUrl_Click(object sender, RoutedEventArgs e)
        {
            var fileUrl = this.txtbFileUrl.Text;
            Clipboard.SetText(fileUrl);

            bool IsFileUrlOnClipboard = Clipboard.ContainsData(DataFormats.Text);
            if (IsFileUrlOnClipboard)
            {
                MessageBox.Show("文件地址已复制到剪切板，可以粘贴后发送给好友");
            }
        }
    }
}
