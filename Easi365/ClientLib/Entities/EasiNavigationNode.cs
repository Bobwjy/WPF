using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ClientLib.Entities
{
    public class EasiNavigationNode : INotifyPropertyChanged
    {
        public EasiNavigationNode()
        {

        }

        public EasiNavigationNode(string name, string fullName, string visibility, string icon)
        {
            this.DisplayName = name;
            this.FullName = fullName;
            this.Visibility = visibility;
            this.Icon = icon;
        }

        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; OnPropertyChanged("DisplayName"); }
        }
        private string fullName;
        public string FullName
        {
            get { return fullName; }
            set { fullName = value; OnPropertyChanged("FullName"); }
        }
        private string visibility;
        public string Visibility
        {
            get { return visibility; }
            set { visibility = value; OnPropertyChanged("Visibility"); }
        }
        private string icon;
        public string Icon
        {
            get { return icon; }
            set { icon = value; OnPropertyChanged("Icon"); }
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

    public class EasiNavigationComparer : IEqualityComparer<EasiNavigationNode>
    {
        public static EasiNavigationComparer Default = new EasiNavigationComparer();

        public bool Equals(EasiNavigationNode x, EasiNavigationNode y)
        {
            return x.DisplayName.Equals(y.DisplayName);
        }

        public int GetHashCode(EasiNavigationNode obj)
        {
            return obj.GetHashCode();
        }
    }
}
