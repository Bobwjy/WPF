using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ClientLib.Entities;

namespace Easi365UI.Models
{
    public class EntryModelBase : PropertyChangedBase, IEntryModel
    {
        #region Field
        protected string _name;
        private bool _isRenamable = false;
        private IEntryModel _parent = null;

        private bool isInEditMode;
       // protected Func<IEntryModel> _parentFunc;

        private string _isLocalCached;
        #endregion

        public IProfile Profile { get; protected set; }
        public bool IsDirectory { get; set; }
        public IEntryModel Parent { get; protected set; }
        //public string Label { get; protected set; }
        public string Name 
        {
            get { return _name; }
            set
            {
                string org = _name;
                _name = value;
                if (!string.IsNullOrEmpty(org) && org.Equals(_name))
                    OnRename(org, _name);
            }
        }
        public string Description { get; protected set; }
        public string FullPath { get; set; }
        //public bool IsRenamable
        //{
        //    get { return _isRenamable; }
        //    set
        //    {
        //        _isRenamable = value;
        //        NotifyOfPropertyChange(() => IsRenamable);
        //    }
        //}

        
        public bool IsInEditMode
        {
            get { return isInEditMode; }
            set { isInEditMode = value; NotifyOfPropertyChange(() => IsInEditMode); }
        }

        public string IsLocalCached
        {
            get { return _isLocalCached; }
            set
            {
                if (value != _isLocalCached)
                {
                    _isLocalCached = value;
                    NotifyOfPropertyChange(() => IsLocalCached);
                }
            }
        }
        /// <summary>
        /// 文件下载状态
        /// </summary>
        public DownloadState DownloadState { get; set; }

        public EntryModelBase()
        {

        }
        public EntryModelBase(IProfile profile)
        {
            this.Profile = profile;

            //下载状态
            this.DownloadState = new DownloadState();
            this.IsInEditMode = false;
        }

        protected void OnRename(string orgName, string newName)
        { }
    }
}
