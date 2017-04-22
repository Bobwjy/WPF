using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Entities
{
    public class SharedUserModel : INotifyPropertyChanged
    {
        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged("UserName");
                }
            }
        }

        private UserRole _userRole;

        public UserRole UserRole
        {
            get { return _userRole; }
            set
            {
                if (_userRole != value)
                {
                    _userRole = value;
                    OnPropertyChanged("UserRole");
                }
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion


        public string LoginName { get; set; }
    }

    public class SharedUserEquality : IEqualityComparer<SharedUserModel>
    {

        public bool Equals(SharedUserModel x, SharedUserModel y)
        {
            return x.UserName == y.UserName &&
                x.LoginName == y.LoginName &&
                x.UserRole == y.UserRole;
        }

        public int GetHashCode(SharedUserModel obj)
        {
            if (obj == null)
                return 0;
            else
               return obj.ToString().GetHashCode();
        }
    }

    public enum UserRole
    {
        View = 0,
        Edit = 1,
        None = 2
    }
}
