using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ClientLib.Entities;
using ClientLib.Services;
using ClientLib.Core;

namespace Easi365UI.Models
{
    public class OrgViewModel : INotifyPropertyChanged
    {
        //临时子节点用，当Expanded时移除此节点，添加子节点
        static readonly OrgViewModel _temp = new OrgViewModel(null);
        //选中的子节点
        private static ObservableCollection<OrgViewModel> _checkedItems = new ObservableCollection<OrgViewModel>();
        //根节点
        static OrgViewModel _rootItem;

        protected static OrgService orgService { get; set; }
        public OrgViewModel() {
            orgService = OrgService.GetInstence(App.spCredentials);
            orgService.Init();
        }

        #region fields&properties
        private bool? _isChecked;
        public bool? IsChecked {
            get { return _isChecked; }
            set {
                SetCheckState(value, true, true);
            }
        }

        private void SetCheckState(bool? value, bool updateChildren, bool updateParent) {
            if (_isChecked != value) {
                _isChecked = value;

                //通知选中项的集合
                if (_isChecked == true) {
                    _checkedItems.Add(this);
                    PropertyChanged(this, new PropertyChangedEventArgs("CheckedItems"));
                } else if (_isChecked == false) {
                    _checkedItems.Remove(this);
                    PropertyChanged(this, new PropertyChangedEventArgs("CheckedItems"));
                }

                PropertyChanged(this, new PropertyChangedEventArgs("IsChecked"));

                if (updateChildren) {
                    if (HasChildren()) {
                        Children.ForEach(c => c.SetCheckState(value, true, false));
                    }
                }
                if (updateParent && _parent != null) {
                    _parent.VerifyState();
                }
            }
        }

        private void VerifyState() {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i) {
                bool? currentState = this.Children[i].IsChecked;
                if (i == 0) {
                    state = currentState;
                } else if (state != currentState) {
                    state = null;
                    break;
                }
            }
            this.SetCheckState(state, false, true);
        }

        private bool _isExpanded;
        public bool IsExpanded {
            get { return _isExpanded; }
            set {
                if (value != _isExpanded) {
                    _isExpanded = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsExpanded"));
                }
                if (!HasChildren()) {
                    Children.Remove(_temp);
                    LoadChildren();
                }
            }
        }

        private object _current;
        public object Current {
            get { return _current; }
            set { _current = value; }
        }

        public int UserId
        {
            get { return ((OrgNode)Current).ID; }
        }

        public string DisplayText {
            //get { return ((DepartmentTreeView.DB.SampleDataSet.DepartmentRow)Current)["DName"].ToString(); }
            get { return ((OrgNode)Current).NodeName; }
        }

        public string Account {
            get { return ((OrgNode)Current).Account; }
        }

        public bool IsUser
        {
            get { return !((OrgNode)Current).NodeIsDept; }
        }

        private OrgViewModel _parent;
        public OrgViewModel Parent {
            get { return _parent; }
            set { _parent = value; }
        }

        private List<OrgViewModel> _children;
        public List<OrgViewModel> Children {
            get { return _children; }
            private set { _children = value; }
        } 
        #endregion

        public List<OrgViewModel> Create() {
            var list = orgService.GetSubDepartments(0);
            OrgViewModel root = new OrgViewModel(null);
            _rootItem = root;
            root.Children.Clear();
            foreach (var item in list) {
                root.Children.Add(new OrgViewModel(item));
            }
            return root.Children;
        }

        private OrgViewModel(object currentObject) {
            Current = currentObject;
            _isChecked = false;
            Children = new List<OrgViewModel>();
            Children.Add(_temp);
        }

        /// <summary>
        /// 初始化，用于设置父节点
        /// </summary>
        private void Init() {
            if (!HasChildren()) return;
            foreach (OrgViewModel child in Children) {
                child.Parent = this;
                child.Init();
            }
            PropertyChanged(this, new PropertyChangedEventArgs("Children"));
        }

        /// <summary>
        /// 加载子节点
        /// </summary>
        private void LoadChildren() {
            if (Current != null) {
                int _pid = ((OrgNode)Current).ID;
                var _nodeIsDept = ((OrgNode)Current).NodeIsDept;
                var list = orgService.GetSubDepartments(_pid, _nodeIsDept);
                foreach (var item in list) {
                    OrgViewModel model = new OrgViewModel(item) { _isChecked = this.IsChecked };
                    if (model.IsChecked == true) {
                        _checkedItems.Add(model);
                        PropertyChanged(this, new PropertyChangedEventArgs("CheckedItems"));
                    }
                    Children.Add(model);
                }
                Init();
            }
        }

        /// <summary>
        /// 判断是否有子节点（逻辑是：如果只有一个临时子节点，说明没有真正的子节点）
        /// </summary>
        /// <returns></returns>
        private bool HasChildren() {
            return !(Children.Count == 1 && Children[0] == _temp);
        }

        public ObservableCollection<OrgViewModel> CheckedItems {
            get {
                return _checkedItems;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
