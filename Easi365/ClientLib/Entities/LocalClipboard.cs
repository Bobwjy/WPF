using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLib.Entities
{
    public class LocalClipboard
    {
        public LocalClipboard()
        {

        }

        public LocalClipboard(string text, bool isDirectory, ClipType clipType)
        {
            this.Text = text;
            this.IsDirectory = isDirectory;
            this.ClipType = clipType;
        }

        public string Text { get; set; }
        public bool IsDirectory { get; set; }
        public ClipType ClipType { get; set; }
    }

    public enum ClipType
    {
        Cut,
        Copy
    }
}
