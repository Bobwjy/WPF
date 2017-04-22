using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ClientLib.Entities;
using Easi365UI.Models;
using Easi365UI.Windows.Controls;

namespace Easi365UI.Extention
{
    /// <summary>
    /// ChooserUser.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseUser : EasiWindow
    {
        public ObservableCollection<CheckedUser> CheckedUsers { get; set; }

        bool _allowMultiple;
        public ChooseUser(bool allowMultiple)
        {
            InitializeComponent();

            this._allowMultiple = allowMultiple;
            this.CheckedUsers = new ObservableCollection<CheckedUser>();

            ObjectDataProvider provider = FindResource("ShareProvider") as ObjectDataProvider;
            List<OrgViewModel> firstLevelItems = provider.Data as List<OrgViewModel>;
            ICollectionView view = CollectionViewSource.GetDefaultView(firstLevelItems);
            OrgViewModel rootItem = view.CurrentItem as OrgViewModel;
            if (rootItem != null) rootItem.CheckedItems.Clear();

            this.DataContext = this;
        }

        private void TreeItemChk_Click(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox.IsChecked == true)
            {
                if (!_allowMultiple && CheckedUsers.Count != 0)
                {
                    checkbox.IsChecked = false;
                    return;
                }
            }

            ObjectDataProvider provider = FindResource("ShareProvider") as ObjectDataProvider;
            List<OrgViewModel> firstLevelItems = provider.Data as List<OrgViewModel>;
            ICollectionView view = CollectionViewSource.GetDefaultView(firstLevelItems);
            OrgViewModel rootItem = view.CurrentItem as OrgViewModel;
            List<OrgViewModel> checkedItems = CollectionViewSource.GetDefaultView(provider) as List<OrgViewModel>;

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

        //选择用户
        private void btnChooseUser_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        //最小化登录窗体
        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        //关闭窗口
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
