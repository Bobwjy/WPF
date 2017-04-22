using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLib.Entities
{
    public class Staff
    {
        public Staff()
        {

        }

        public Staff(string account, string username, string phone,string dept)
        {
            this.Account = account;
            this.UserName = username;
            this.TelPhone = phone;
            this.Dept = dept;
        }

        public int ID { get; set; }
        public string UserName { get; set; }
        public string Account { get; set; }
        public string TelPhone { get; set; }
        public string Dept { get; set; }
        public int CurrentDeptID { get; set; }
        public int NewDeptID { get; set; }
        public string Modified { get; set; }
    }
}
