using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ClientLib.Entities
{
    [Serializable]
    [XmlRoot("ServerItem")]
    public class LocalFileInfo
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string ServerRelativePath { get; set; }
        //是否为个人文档库的文件 （特殊文件 关注的文件，共享的文件）
        public bool IsNormalFile { get; set; }

        public int Version { get; set; }
    }
}
