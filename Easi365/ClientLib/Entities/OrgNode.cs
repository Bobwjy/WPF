using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLib.Entities
{
    public class OrgNode
    {
        public int ID { get; set; }
        public string NodeName { get; set; }
        public bool NodeIsDept { get; set; }
        public string Account { get; set; }
        public string ParentDept { get; set; }
        public int ParentID { get; set; }
    }
}
