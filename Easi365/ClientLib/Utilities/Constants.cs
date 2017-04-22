using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLib.Utilities
{
    public class Constants
    {
        public static class Sharing
        {
            //本地服务器LoginName前缀
            public static readonly string LoginNamePrefix = "i:0#.w|";
            public static readonly string Office365LoginNamePrefix = "i:0#.f|membership|";
        }

        public static class SpaceSite
        {
            public static readonly string CooSpace = "CooSpace";
            public static readonly string Depeartment = "DeptSpace";
            public static readonly string CooSpaceListTitle = "CooSpaceList";
            public static readonly string DeptSpaceListTitle = "DeptList";
        }

        public static List<string> NoteExtensions = new List<string>() 
        { 
            "asp", "aspx", "java", "cs", "xml", "php", "js"
        };
        public static List<string> FileExtensions = new List<string>() 
        {
            "doc", "docx", "xls", "xlsx", "ppt", "pptx", "mdb", "accdb", "pub", "pst",
            "jpg", "jpeg", "png", "bmp", "gif", "ico",
            "mp3", "mp4", "mpg", "mpeg", "wma", "avi", "cap", "eps",
            "rar", "zip",
            "fax", 
            "swf", "fla", "pdf", "psd",
            "dll", "exe", "ini", "jar", "set",
            "htm", "html", "css",
            "txt",
            "iso", "img"
        };
    }
}
