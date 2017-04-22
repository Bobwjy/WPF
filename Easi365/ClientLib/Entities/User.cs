using Microsoft.SharePoint.Client.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLib.Entities
{
    public class UserInfo
    {
        private PersonProperties _roperties;
        public string Account { get; set; }
        public string DisplayName { get; set; }
        public string JobTitle { get; set; }
        public string PictureUrl { get; set; }
    }
    
    public class CheckedUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Account { get; set; }
    }

    public class CheckedUserComparer : IEqualityComparer<CheckedUser>
    {
        public static CheckedUserComparer Default = new CheckedUserComparer();

        public bool Equals(CheckedUser x, CheckedUser y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(CheckedUser obj)
        {
            return obj.GetHashCode();
        }
    }
}
