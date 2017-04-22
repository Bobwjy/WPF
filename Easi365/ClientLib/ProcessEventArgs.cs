using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLib
{
    public class ProcessEventArgs : EventArgs
    {
        public string CurrentFileName { get; set; }
        public int UploadedSize { get; set; }

        public ProcessEventArgs(string name, int size)
        {
            this.CurrentFileName = name;
            this.UploadedSize = size;
        }
    }
}
