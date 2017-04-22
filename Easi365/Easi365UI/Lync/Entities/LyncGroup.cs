using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using ClientLib.Core;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Group;

namespace Easi365UI.Lync
{
    public class LyncGroup : INotifyPropertyChanged
    {
        Dispatcher _dispatcher;
        public LyncGroup(Group group, Dispatcher dispatcher)
        {
            _Group = group;
            _Group.NameChanged += _Group_NameChanged;
            _Group.ContactAdded += _Group_ContactAdded;
            _Group.ContactRemoved += _Group_ContactRemoved;
            _dispatcher = dispatcher;

            this.Name = group.Name;
            this.ContractList = new ObservableCollection<LyncContract>();
            this.LoadContacts();

            if (group.Type == GroupType.DistributionGroup)
            {
                _DGroup = (DistributionGroup)group;
            }
            else if (group.Type == GroupType.CustomGroup)
            {
                _CGroup = (CustomGroup)group;
            }
        }

        public Group _Group;               //组
        public CustomGroup _CGroup;        //自定义组
        public DistributionGroup _DGroup;  //？？？组

        //名称
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        //联系人列表
        private ObservableCollection<LyncContract> contractList;
        public ObservableCollection<LyncContract> ContractList
        {
            get { return contractList; }
            set { contractList = value; OnPropertyChanged("ContractList"); }
        }

        //加载分组下的联系人
        void LoadContacts()
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    this.ContractList.Clear();

                    ContactCollection contacts = null;
                    if (_Group.Type == GroupType.DistributionGroup)
                    {
                        _DGroup.BeginGetAllMembers(
                        (ar) =>
                        {
                            try
                            {
                                contacts = _DGroup.EndGetAllMembers(ar);
                                foreach (var contact in contacts)
                                {
                                    this.ContractList.Add(new LyncContract(contact, this, _dispatcher));
                                }
                            }
                            catch (LyncClientException lce)
                            {
                                Logging.Add("Lync Exception - LoadContacts", lce);
                            }
                        },
                        null);
                    }
                    else
                    {
                        foreach (var contact in _Group)
                        {
                            this.ContractList.Add(new LyncContract(contact, this, _dispatcher));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Add("Lync Exception - System Exception - LoadContacts", ex);
                }
            }), null);
        }

        #region 重命名组
        public void RenameGroup(string name)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    if (_CGroup != null)
                    {
                        _CGroup.BeginRename(name, RenameGroup_Callback, null);
                    }
                }
                catch (LyncClientException lce)
                {
                    Logging.Add("Lync Exception - RenameGroup", lce);
                }
            }), null);
        }
        void RenameGroup_Callback(IAsyncResult ar)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    _CGroup.EndRename(ar);
                }
                catch (LyncClientException lce)
                {
                    Logging.Add("Lync Exception - RenameGroup_Callback", lce);
                }
            }), null);
        }

        //成功重命名组
        void _Group_NameChanged(object sender, GroupNameChangedEventArgs e)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                this.Name = e.NewName;
            }), null);
        }
        #endregion

        #region 添加联系人到自定义组
        public void AddContract(Contact contact)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    _Group.BeginAddContact(contact, AddContract_Callback, null);
                }
                catch (LyncClientException lce)
                {
                    Logging.Add("Lync Exception - AddContract", lce);
                }
            }), null);
        }
        void AddContract_Callback(IAsyncResult ar)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    _Group.EndAddContact(ar);
                }
                catch (LyncClientException lce)
                {
                    Logging.Add("Lync Exception - AddContract_Callback", lce);
                }
            }), null);
        }

        //成功添加联系人
        void _Group_ContactAdded(object sender, GroupMemberChangedEventArgs e)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    LyncContract contract = new LyncContract(e.Contact, this, _dispatcher);

                    if (!this.ContractList.Contains<LyncContract>(contract, LyncContractComparer.Default))
                    {
                        this.ContractList.Add(contract);
                    }
                }
                catch (LyncClientException lce)
                {
                    Logging.Add("Lync Exception - _Group_ContactAdded", lce);
                }
            }), null);
        }
        #endregion

        #region 从组中移除联系人
        public void RemoveContract(Contact contact)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    _Group.BeginRemoveContact(contact, RemoveContract_Callback, null);
                }
                catch (LyncClientException lce)
                {
                    Logging.Add("Lync Exception - RemoveContract", lce);
                }
            }), null);
        }
        void RemoveContract_Callback(IAsyncResult ar)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    _Group.EndRemoveContact(ar);
                }
                catch (LyncClientException lce)
                {
                    Logging.Add("Lync Exception - RemoveContract_Callback", lce);
                }
            }), null);
        }

        //成功移除联系人
        void _Group_ContactRemoved(object sender, GroupMemberChangedEventArgs e)
        {
            _dispatcher.Invoke(new Action(delegate()
            {
                try
                {
                    var contract = this.ContractList.Where(m => m.Uri == e.Contact.Uri).FirstOrDefault();
                    if (contract != null)
                    {
                        this.ContractList.Remove(contract);
                    }
                }
                catch (LyncClientException lce)
                {
                    Logging.Add("Lync Exception - _Group_ContactRemoved", lce);
                }
            }), null);
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
    }

    public class LyncGroupComparer : IEqualityComparer<LyncGroup>
    {
        public static LyncGroupComparer Default = new LyncGroupComparer();

        public bool Equals(LyncGroup x, LyncGroup y)
        {
            return x.Name.Equals(y.Name);
        }

        public int GetHashCode(LyncGroup obj)
        {
            return obj.GetHashCode();
        }
    }
}
