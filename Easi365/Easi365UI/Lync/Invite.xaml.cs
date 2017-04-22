using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Group;

namespace Easi365UI.Lync
{
    /// <summary>
    /// Invite.xaml 的交互逻辑
    /// </summary>
    public partial class Invite : Window
    {
        LyncClient _lyncClient;
        Dispatcher _dispatcher;

        public Invite()
        {
            InitializeComponent();
            _dispatcher = Dispatcher.CurrentDispatcher;

            this.ContractList = new ObservableCollection<LyncContract>();
            this.DataContext = this;
        }

        public string Uri;

        public ObservableCollection<LyncContract> ContractList { get; set; }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            try
            {
                _lyncClient = LyncClient.GetClient();

                if (_lyncClient.State == ClientState.SignedIn)
                {
                    LoadContracts();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        #region 联系人

        //读取联系人
        private void LoadContracts()
        {
            GroupCollection groups = _lyncClient.ContactManager.Groups;
            Thread.Sleep(1000);
            for (int i = 0; i < groups.Count; i++)
            {
                Group group = groups[i];
                ContactCollection contacts = null;
                if (group.Type == GroupType.DistributionGroup)
                {
                    var dGroup = (DistributionGroup)group;
                    dGroup.BeginGetAllMembers(
                    (ar) =>
                    {
                        try
                        {
                            contacts = dGroup.EndGetAllMembers(ar);
                            foreach (var contact in contacts)
                            {
                                this.ContractList.Add(new LyncContract(contact, _dispatcher));
                            }
                        }
                        catch { }
                    },
                    null);
                }
                else
                {
                    foreach (var contact in group)
                    {
                        this.ContractList.Add(new LyncContract(contact, _dispatcher));
                    }
                }
            }
        }

        #endregion

        #region 选择联系人

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LyncContract contract = listContact.SelectedItem as LyncContract;
            Uri = contract.Uri;
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Uri = string.Empty;
            if (listContact.SelectedItem != null)
            {
                LyncContract contract = listContact.SelectedItem as LyncContract;
                Uri = contract.Uri;
            }
            this.Close();
        }

        #endregion

        #region 取消

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
