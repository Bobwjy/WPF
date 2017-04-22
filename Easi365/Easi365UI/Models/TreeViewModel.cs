using ClientLib;
using ClientLib.Core;
using ClientLib.Services;
using ClientLib.Utilities;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easi365UI.Models
{
    public class TreeViewModel : INotifyPropertyChanged
    {
        //临时子节点用，当Expanded时移除此节点，添加子节点
        public static readonly TreeViewModel _temp = new TreeViewModel(null);
        string folderIcoUrl = "/assets/ui/small/folder.png";

        public TreeViewModel(string nodeName, CoreManager.SpaceCategory topLevel = CoreManager.SpaceCategory.Company, string spaceUri = "",
            bool isFolder = false, bool isLoadChildFolder = false, string folderRelativeUrl = "", string icoUrl = "", ServerSide serverSide = null, List remoteList = null)
        {
            NodeName = nodeName;
            SpaceUri = spaceUri;
            FolderRelativeUrl = folderRelativeUrl;
            IsFolder = isFolder;
            IsLoadChildFolder = isLoadChildFolder;
            IcoUrl = icoUrl;
            TopLevel = topLevel;
            ServerSide = serverSide;
            RemoteList = remoteList;
            Children = new List<TreeViewModel>();
            Children.Add(_temp);
        }

        #region Properties
        public string NodeName { get; set; }
        public string SpaceUri { get; private set; }
        public bool IsFolder { get; private set; }
        public bool IsLoadChildFolder { get; private set; }
        public string FolderRelativeUrl { get; set; }
        public string IcoUrl { get; set; }
        public CoreManager.SpaceCategory TopLevel { get; set; }
        public List<TreeViewModel> Children { get; private set; }
        public ServerSide ServerSide { get; set; }
        public List RemoteList { get; set; }
        public bool IsInitiallySelected { get; private set; }

        bool? _isChecked = false;
        TreeViewModel _parent;

        #region IsChecked

        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked) return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
            {
                this.SetIsChecked(_isChecked, true, false);
            }

            //if (updateChildren && _isChecked.HasValue) Children.ForEach(c => {
            //    if (c != null)
            //    {
            //        c.SetIsChecked(_isChecked, true, false);
            //    }
            //});
            //if (updateParent && _parent != null) _parent.VerifyCheckedState();

            NotifyPropertyChanged("IsChecked");
        }

        void VerifyCheckedState()
        {
            bool? state = null;

            for (int i = 0; i < Children.Count; ++i)
            {
                bool? current = Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }

            SetIsChecked(state, false, true);
        }

        #endregion

        #endregion

        void Initialize()
        {
            foreach (TreeViewModel child in Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        public static List<TreeViewModel> GetCheckedItems(TreeViewModel firstLevelItem)
        {
            var checkedItems = new List<TreeViewModel>();
            ProcessNode(firstLevelItem, checkedItems);
            return checkedItems;
        }

        private static void ProcessNode(TreeViewModel node, List<TreeViewModel> checkedItems)
        {
            if (node == null) return;
            foreach (var child in node.Children)
            {
                if (child != null && (child.IsChecked ?? true))
                    checkedItems.Add(child);

                ProcessNode(child, checkedItems);
            }
        }

        #region INotifyPropertyChanged Members

        void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
