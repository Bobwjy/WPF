using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easi365UI.Lync
{
    public class User
    {
        private User()
        {

        }

        public User(string uri, string username, string password)
        {
            this.Uri = uri;
            this.Username = username;
            this.Password = password;
        }
        
        public string Uri { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
