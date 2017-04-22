using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using ClientLib.Core;
using Easi365UI.Lync;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Group;

namespace Easi365UI.Models
{
    public class LyncViewModel : INotifyPropertyChanged
    {
        LyncClient _lyncClient;
        Dispatcher _dispatcher;
        public LyncViewModel()
        {
            this.GroupList = new ObservableCollection<LyncGroup>();
            this.GroupNameList = new ObservableCollection<string>();

            this.SearchContractCount = 9999;
            this.SearchContractList = new ObservableCollection<LyncContract>();
        }

        public void InitClient(LyncClient lyncClient, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;

            _lyncClient = lyncClient;
            _lyncClient.ContactManager.GroupAdded += ContactManager_GroupAdded;
            _lyncClient.ContactManager.GroupRemoved += ContactManager_GroupRemoved;

            this.LoadGroups();
        }

        //搜索到的联系人数量（值为-1时未“正在搜索”）
        private int searchContractCount;
        public int SearchContractCount
        {
            get { return searchContractCount; }
            set { searchContractCount = value; OnPropertyChanged("SearchContractCount"); }
        }

        //搜索到的联系人列表
        private ObservableCollection<LyncContract> searchContractList;
        public ObservableCollection<LyncContract> SearchContractList
        {
            get { return searchContractList; }
            set { searchContractList = value; OnPropertyChanged("SearchContractList"); }
        }

        //自定义分组名称列表
        private ObservableCollection<string> groupNameList;
        public ObservableCollection<string> GroupNameList
        {
            get { return groupNameList; }
            set { groupNameList = value; OnPropertyChanged("GroupNameList"); }
        }

        //分组列表
        private ObservableCollection<LyncGroup> groupList;
        public ObservableCollection<LyncGroup> GroupList
        {
            get { return groupList; }
            set { groupList = value; OnPropertyChanged("GrouptList"); }
        }

        //加载联系人分组
        void LoadGroups()
        {
            this.GroupList.Clear();

            var groups = _lyncClient.ContactManager.Groups;
            foreach (var group in groups)
            {
                try
                {
                    this.GroupList.Add(new LyncGroup(group, _dispatcher));
                    if (group.Type == GroupType.CustomGroup)
                    {
                        this.GroupNameList.Add(group.Name);
                    }
                }
                catch (LyncClientException lce)
                {
                    Logging.Add("Lync Exception - " + group.Name, lce);
                }
            }
        }

        #region 搜索联系人
        public void SearchContracts(string key)
        {
            try
            {
                _lyncClient.ContactManager.BeginSearch(
                    key,
                    SearchProviders.GlobalAddressList,
                    SearchFields.AllFields,
                    SearchOptions.ContactsOnly,
                    500,
                    SearchContracts_Callback,
                    new object[] { _lyncClient.ContactManager }
                );

                _dispatcher.Invoke(new Action(delegate()
                {
                    this.SearchContractCount = -1;
                    this.SearchContractList.Clear();
                }), null);
            }
            catch (SearchException se)
            {
                Logging.Add("Lync Exception - Search Exception - SearchContracts", se);
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - SearchContracts", lce);
            }
        }
        void SearchContracts_Callback(IAsyncResult r)
        {
            try
            {
                object[] asyncState = (object[])r.AsyncState;
                ContactManager contactManager = (ContactManager)asyncState[0];

                SearchResults result = contactManager.EndSearch(r);
                _dispatcher.Invoke(new Action(delegate()
                {
                    var contracts = result.Contacts.Where(m => m.Uri != _lyncClient.Self.Contact.Uri);
                    if (contracts == null || contracts.Count() == 0)
                    {
                        this.SearchContractCount = 0;
                    }
                    else
                    {
                        this.SearchContractCount = contracts.Count();
                        foreach (var contact in contracts)
                        {
                            var tempContract = this.SearchContractList.Where(m => m.Uri == contact.Uri).FirstOrDefault();
                            if (tempContract == null)
                            {
                                this.SearchContractList.Add(new LyncContract(contact, _dispatcher));
                            }
                        }
                    }
                }), null);
            }
            catch (SearchException se)
            {
                Logging.Add("Lync Exception - Search Exception - SearchContracts_Callback", se);
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - SearchContracts_Callback", lce);
            }
            catch (Exception ex)
            {
                Logging.Add("Lync Exception - System Exception - SearchContracts_Callback", ex);
            }
        }

        //根据uri查找联系人
        public LyncContract GetContractByUri(string uri)
        {
            try
            {
                Contact contact = _lyncClient.ContactManager.GetContactByUri(uri);
                if (contact != null)
                {
                    return new LyncContract(contact, _dispatcher);
                }
            }
            catch (SearchException se)
            {
                Logging.Add("Lync Exception - Search Exception - GetContractByUri", se);
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - SearchContracts", lce);
            }

            return null;
        }
        #endregion

        #region 从联系人列表中删除
        public void RemoveContract(Contact contact)
        {
            try
            {
                _lyncClient.ContactManager.BeginRemoveContactFromAllGroups(contact, RemoveContract_Callback, null);
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - RemoveContract", lce);
            }
        }
        void RemoveContract_Callback(IAsyncResult ar)
        {
            try
            {
                _lyncClient.ContactManager.EndRemoveContactFromAllGroups(ar);
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - RemoveContract_Callback", lce);
            }
        }
        #endregion

        #region 添加组
        public void AddGroup(string name)
        {
            try
            {
                _lyncClient.ContactManager.BeginAddGroup(name, AddGroup_Callback, null);
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - AddGroup", lce);
            }
        }
        void AddGroup_Callback(IAsyncResult ar)
        {
            try
            {
                _lyncClient.ContactManager.EndAddGroup(ar);
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - AddGroup_Callback", lce);
            }
        }

        //成功添加组
        void ContactManager_GroupAdded(object sender, GroupCollectionChangedEventArgs e)
        {
            LyncGroup group = new LyncGroup(e.Group, _dispatcher);
            _dispatcher.Invoke(new Action(delegate()
            {
                if (!this.GroupList.Contains<LyncGroup>(group, LyncGroupComparer.Default))
                {
                    this.GroupList.Add(group);
                }
                if (e.Group.Type == GroupType.CustomGroup && !this.GroupNameList.Contains(group.Name))
                {
                    this.GroupNameList.Add(group.Name);
                }
            }), null);
        }
        #endregion

        #region 删除组
        public void RemoveGroup(LyncGroup group)
        {
            try
            {
                _lyncClient.ContactManager.BeginRemoveGroup(group._Group, RemoveGroup_Callback, null);
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - RemoveGroup", lce);
            }
        }
        void RemoveGroup_Callback(IAsyncResult ar)
        {
            try
            {
                _lyncClient.ContactManager.EndRemoveGroup(ar);
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - RemoveGroup_Callback", lce);
            }
        }

        //成功删除组
        void ContactManager_GroupRemoved(object sender, GroupCollectionChangedEventArgs e)
        {
            try
            {
                _dispatcher.Invoke(new Action(delegate()
                {
                    var group = this.GroupList.Where(m => m.Name == e.Group.Name).FirstOrDefault();
                    if (group != null)
                    {
                        this.GroupList.Remove(group);
                    }
                    if (this.GroupNameList.Contains(group.Name))
                    {
                        this.GroupNameList.Remove(group.Name);
                    }
                }), null);
            }
            catch (LyncClientException lce)
            {
                Logging.Add("Lync Exception - ContactManager_GroupRemoved", lce);
            }
            catch (Exception ex)
            {
                Logging.Add("Lync Exception - System Exception - ContactManager_GroupRemoved", ex);
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        //Lync相关菜单命令
        public static RoutedUICommand SendMessageCommand = new RoutedUICommand();
        public static RoutedUICommand AddContractCommand = new RoutedUICommand();
        public static RoutedUICommand CopyContractCommand = new RoutedUICommand();
        public static RoutedUICommand MoveContractCommand = new RoutedUICommand();
        public static RoutedUICommand RemoveContractFromGroupCommand = new RoutedUICommand();
        public static RoutedUICommand RemoveContractCommand = new RoutedUICommand();
        public static RoutedUICommand AddContractToListCommand = new RoutedUICommand();
        public static RoutedUICommand AddContractToGroupCommand = new RoutedUICommand();
        public static RoutedUICommand CopyContractToGroupCommand = new RoutedUICommand();
        public static RoutedUICommand MoveContractToGroupCommand = new RoutedUICommand();

        public static RoutedUICommand AddGroupCommand = new RoutedUICommand();
        public static RoutedUICommand RenameGroupCommand = new RoutedUICommand();
        public static RoutedUICommand RemoveGroupCommand = new RoutedUICommand();

        //组织架构中发送消息
        public static RoutedUICommand SendMessageOnOrganizationCommand = new RoutedUICommand();
    }
}
