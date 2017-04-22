using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Easi365UI.Entities
{
    public class CompanyLeftMenu
    {
        public CompanyLeftMenu()
        {
            this.Children = new ObservableCollection<CompanyLeftMenu>();
        }

        public string NodeName { get; set; }
        public string Url { get; set; }
        public string WebUrl { get; set; }
        public bool IsDoc { get; set; }
        public string IcoUrl { get; set; }

        public ObservableCollection<CompanyLeftMenu> Children { get; set; }
    }
}
