using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ClientLib.Entities
{
    public class LocalFile : INotifyPropertyChanged
    {
        public LocalFile(FileSystemInfo file)
        {
            Info = file;
            Name = file.Name;
            FullName = file.FullName;
            Modified = file.LastAccessTime;
            Type = IsDirectory ? "文件夹" : file.Extension + " 文件";
            Size = IsDirectory ? 0 : (int)new FileInfo(file.FullName).Length;
            IsInEditMode = false;
        }

        public FileSystemInfo Info { get; set; }
        
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        public string FullName { get; set; }

        private string type;
        public string Type
        {
            get { return type; }
            set { type = value; OnPropertyChanged("Type"); }
        }

        private int size;
        public int Size
        {
            get { return size; }
            set { size = value; OnPropertyChanged("Size"); }
        }

        private DateTime modified;
        public DateTime Modified
        {
            get { return modified; }
            set { modified = value; OnPropertyChanged("Modified"); }
        }
        
        public bool IsDirectory
        {
            get
            {
                return (Info.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
            }
        }

        private bool isInEditMode;
        public bool IsInEditMode
        {
            get { return isInEditMode; }
            set { isInEditMode = value; OnPropertyChanged("IsInEditMode"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public string GoDown()
        {
            if ((Info.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return Info.FullName;
            }
            return null;
        }

        public string GoUp()
        {
            if ((Info.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return new DirectoryInfo(Info.FullName).Parent.FullName;
            }
            return null;
        }

        public void OpenItem()
        {
            Process.Start(Info.FullName);
        }
    }
}
