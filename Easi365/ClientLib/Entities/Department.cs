using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ClientLib.Utilities;

namespace ClientLib.Entities
{
    public class Department : INotifyPropertyChanged
    {
        private string id;
        public string ID
        {
            get { return id; }
            set { id = value; OnPropertyChanged("ID"); }
        }
        private string deptName;
        public string DeptName
        {
            get { return deptName; }
            set { deptName = value; OnPropertyChanged("DeptName"); }
        }
        private string parentId;
        public string ParentID
        {
            get { return parentId; }
            set { parentId = value; OnPropertyChanged("ParentID"); }
        }
        private string parentName;
        public string ParentName
        {
            get { return parentName; }
            set { parentName = value; OnPropertyChanged("ParentName"); }
        }
        private string spaceUri;
        public string SpaceUri
        {
            get { return spaceUri; }
            set { spaceUri = value; OnPropertyChanged("SpaceUri"); }
        }

        public IList<CheckedUser> SpaceManager { get; set; }
        public IList<CheckedUser> OriginalManager { get; set; }

        public Department ParentDept { get; set; }
        public ObservableCollection<Department> Depts { get; set; }
        public string Modified { get; set; }

        public Department()
        {
            this.Depts = new ObservableCollection<Department>();
        }

        public Department(string deptName, IList<CheckedUser> managers,Department pDept)
            : this()
        {
            this.DeptName = deptName;
            this.SpaceManager = managers;
            this.ParentDept = pDept;
            this.OriginalManager = managers;
            this.SpaceUri = Guid.NewGuid().ToString();
        }

        public ObservableCollection<Department> FindDirectParent(ObservableCollection<Department> _nodes)
        {
            ObservableCollection<Department> ret = new ObservableCollection<Department>();
            if (_nodes.Contains(this) == true)
            {
                ret = _nodes;
            }
            else
            {
                foreach (Department item in _nodes)
                {
                    if (item.Depts != null && item.Depts.Contains(this) == true)
                        ret = FindDirectParent(item.Depts);
                }
            }
            return ret;
        }

        public ObservableCollection<Department> InsertSubDept(ObservableCollection<Department> _depts, Department _newSubDept)
        {
            foreach (Department node in _depts)
            {
                if (node.ID == this.ID)
                {
                    node.Depts.Add(_newSubDept);
                    break;
                }
                else
                {
                    if (node.Depts.Count > 0) InsertSubDept(node.Depts, _newSubDept);
                }
            }
            return _depts;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
