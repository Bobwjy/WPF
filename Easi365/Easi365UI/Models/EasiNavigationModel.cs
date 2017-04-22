using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLib.Core;
using ClientLib.Entities;

namespace Easi365UI.Models
{
    public class EasiNavigationModel : INotifyPropertyChanged
    {
        string RootFolder = string.Empty;

        public EasiNavigationModel(string rootFolder)
        {
            this.RootFolder = rootFolder;
            this.Directory = RootFolder;
            this.NodeList = new ObservableCollection<EasiNavigationNode>();
        }

        public Stack<string> BackwardNavigations = new Stack<string>();
        public Stack<string> ForwardNavigations = new Stack<string>();

        public bool HasEllipsis { get; set; }

        private string directory;
        public string Directory
        {
            get { return directory; }
            set { directory = value; OnPropertyChanged("Directory"); }
        }

        private ObservableCollection<EasiNavigationNode> nodeList;
        public ObservableCollection<EasiNavigationNode> NodeList
        {
            get { return nodeList; }
            set { nodeList = value; OnPropertyChanged("NodeList"); }
        }

        public void Refresh(object path)
        {

            this.HasEllipsis = false;

            if (path != null && path is string)
                Directory = (string)path;

            if (System.IO.Directory.Exists(Directory))
            {
                NodeList.Clear();
                NodeList.Add(new EasiNavigationNode("本地空间", RootFolder, "Visible", ""));
                DirectoryInfo dir = new DirectoryInfo(Directory);
                try
                {
                    IList<DirectoryInfo> dics = new List<DirectoryInfo>();
                    do
                    {
                        if (dir.FullName == RootFolder) break;
                        dics.Add(dir);
                        dir = dir.Parent;
                    }
                    while (true);

                    if (dics.Count == 0) return;
                    for (int i = dics.Count - 1; i >= 0; i--)
                        NodeList.Add(new EasiNavigationNode(dics[i].Name, dics[i].FullName, "Visible", "Assets/Images/tag.png"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void RemoveNode()
        {
            if (this.NodeList.Count > 2)
            {
                this.NodeList.RemoveAt(2);
                this.HasEllipsis = true;
            }
        }

        public void AddEllipsis()
        {
            EasiNavigationNode node = new EasiNavigationNode("...", "", "", "Assets/Images/tag.png");
            if (!this.NodeList.Contains<EasiNavigationNode>(node, EasiNavigationComparer.Default))
            {
                this.NodeList.Insert(1, node);
            }
        }

        public void RemoveEllipsis()
        {
            EasiNavigationNode node = new EasiNavigationNode("...", "", "", "Assets/Images/tag.png");
            if (!this.HasEllipsis && this.NodeList.Contains<EasiNavigationNode>(node, EasiNavigationComparer.Default))
                this.NodeList.RemoveAt(1);
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
