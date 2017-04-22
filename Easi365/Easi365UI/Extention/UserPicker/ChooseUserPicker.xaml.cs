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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClientLib.Entities;

namespace Easi365UI.Extention
{
    /// <summary>
    /// UserPicker.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseUserPicker : UserControl
    {
        public ObservableCollection<CheckedUser> Users
        {
            get { return (ObservableCollection<CheckedUser>)GetValue(UsersProperty); }
            set { SetValue(UsersProperty, value); }
        }
        public static readonly DependencyProperty UsersProperty =
            DependencyProperty.Register(
            "Users",
            typeof(ObservableCollection<CheckedUser>),
            typeof(ChooseUserPicker),
            new PropertyMetadata(null));

        public bool AllowMultiple
        {
            get { return (bool)GetValue(AllowMultipleProperty); }
            set { SetValue(AllowMultipleProperty, value); }
        }
        public static readonly DependencyProperty AllowMultipleProperty =
            DependencyProperty.Register(
            "AllowMultiple",
            typeof(bool),
            typeof(ChooseUserPicker),
            new PropertyMetadata(false));

        public ChooseUserPicker()
        {
            InitializeComponent();

            this.DataContext = this;
            this.Users = new ObservableCollection<CheckedUser>();
        }

        private void UserPicker_Loaded(object sender, RoutedEventArgs e)
        {
            this.CheckedUsersLst.Height = AllowMultiple ? 44 : 24;
        }

        private void btnChooseUser_Click(object sender, RoutedEventArgs e)
        {
            ChooseUser dialog = new ChooseUser(this.AllowMultiple);
            dialog.ShowDialog();
            if (dialog.DialogResult ?? false)
            {
                if (this.AllowMultiple)
                {
                    foreach (var checkedUser in dialog.CheckedUsers)
                    {
                        if (!this.Users.Contains<CheckedUser>(checkedUser, CheckedUserComparer.Default))
                            this.Users.Add(checkedUser);
                    }
                }
                else if(dialog.CheckedUsers.Count == 1)
                {
                    this.Users = dialog.CheckedUsers;
                }
            }
        }

        private void btnRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32((sender as Button).Tag);
            var user = this.Users.FirstOrDefault(m => m.Id == id);
            if (user != null)
            {
                this.Users.Remove(user);
            }
        }
    }
}
