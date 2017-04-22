using ClientLib.Core;
using ClientLib.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLib.Entities
{
    public class Spaces
    {
        public string ID { get; set; }
        public string SpaceTitle { get; set; }
        public string Applicant { get; set; }
        public string Account { get; set; }
        public bool IsCreated { get; set; }
        public IList<CheckedUser> SpaceMember { get; set; }
        public IList<CheckedUser> SpaceAdmin { get; set; }
        public IList<CheckedUser> OriginalManager { get; set; }
        public string SpaceUri { get; set; }
        public string Desc { get; set; }
        public string Status { get; set; }
        public CoreManager.SpaceCategory SpaceCategory { get; set; }

        public string SpaceAdmins
        {
            get
            {
                return string.Join(";", SpaceAdmin.Select(m => m.UserName).ToArray());
            }
        }

        public string SpaceMembers
        {
            get
            {
                return string.Join(";", SpaceMember.Select(m => m.UserName).ToArray());
            }
        }

        public bool HasPermission(OrgService service)
        {
            var user = CoreManager.ConfigManager.Settings.CurrentUserName;
            if (user.IndexOf('\\') > -1)
                user = user.Split(new char[] { '\\' })[1];

            //string currentUser = CoreManager.ConfigManager.Settings.CurrentUserName.ToUpper();
            string currentUser = user.ToUpper();

            if (currentUser == "ADMINISTRATOR") return true;

            bool hasPermission = false;
            bool isAdmin = this.SpaceAdmin.Where(m => m.Account.ToUpper() == currentUser).FirstOrDefault() != null;
            if (this.SpaceCategory == CoreManager.SpaceCategory.CooSpace)
            {
                bool isSpaceMember = this.SpaceMember.Where(m => m.Account.ToUpper() == currentUser).FirstOrDefault() != null;
                if (isSpaceMember || isAdmin)
                    hasPermission = true;
            }
            else if (this.SpaceCategory == CoreManager.SpaceCategory.DeptSpace)
            {
                if (service.IsInDepeartment(this.ID, CoreManager.ConfigManager.Settings.ID) || isAdmin)
                    hasPermission = true;
            }

            return hasPermission;
        }
    }
}
