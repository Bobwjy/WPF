using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLib.Entities
{
    public class Editor
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string LoginName { get; set; }
        public string Email { get; set; }
        public string Sip { get; set; }
    }
}
