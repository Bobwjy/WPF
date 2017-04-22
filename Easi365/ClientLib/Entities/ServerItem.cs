using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using System.Xml.Serialization;

namespace ClientLib.Entities
{
    public class ServerItem
    {
        /// <summary>
        /// 列表条目ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 列表条目Guid
        /// </summary>
        public string UniqueId { get; set; }
        /// <summary>
        /// 文件名（带有扩展名）/目录名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 相对服务器路径
        /// </summary>
        public string ServerRelativeUrl { get; set; }
        /// <summary>
        /// 相对文档库路径
        /// </summary>
        public string LibRelativeUrl { get; set; }
        /// <summary>
        /// 类型（文件/文件夹）
        /// </summary>
        public FileOrFolderType ItemType { get; set; }
        /// <summary>
        /// 内部版本（owshiddenversion字段）
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 父文件夹的列表条目ID，如果当前文件/文件夹位于列表根文件夹中，则返回0
        /// </summary>
        public int ParentFolderId { get; set; }

        public ServerItem ParentFolder { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime Modified { get; set; }
        /// <summary>
        /// 共享（只有自己能看到该文件\文件夹 值为1，与其他人共享后 值大于1）
        /// </summary>
        public int SharedWith { get; set; }

        /// <summary>
        /// 文件的大小 文件夹永远是0
        /// </summary>
        public int FileSize { get; set; }

        #region Documents Shared With Me
        public string Title { get; set; }
        public string FileLeafRef { get; set; }
        public string Path { get; set; }
        public Editor[] Editor { get; set; }

        public string Editors
        {
            get
            {
                return string.Join("; ", Editor.Select(m => m.Value));
            }
        }

        public string SiteId { get; set; }
        public string SiteUrl { get; set; }
        public string ListId { get; set; }
        public int ListItemId { get; set; }
        public int FSObjType { get; set; }
        #endregion

        public ServerItem() { }

        public ServerItem(ListItem item)
        {
            this.Id = item.Id;
            this.Name = Convert.ToString(item["FileLeafRef"]);
            this.ServerRelativeUrl = Convert.ToString(item["FileRef"]);
            this.ItemType = item.FileSystemObjectType == FileSystemObjectType.Folder ?
                FileOrFolderType.Folder : FileOrFolderType.File;
            this.Version = Convert.ToInt32(item["owshiddenversion"]);
            this.Modified = Convert.ToDateTime(item["Modified"]).ToLocalTime();
            this.UniqueId = Convert.ToString(item["UniqueId"]);
            //if (item["SharedWith"]!=null)
            //    this.SharedWith = Convert.ToInt32(item["SharedWith"]);

            this.FileSize = GetFileSize(item);
        }

        private int GetFileSize(ListItem itm)
        {
            if (this.ItemType == FileOrFolderType.Folder)
                return 0;

            if (itm.File != null)
                return (int)itm.File.Length;

            return 0;
        }

        //private ServerItem GetParentFolder(Folder parentFodler)
        //{
            
        //}

        /// <summary>
        /// 构造（通过CSOM）
        /// </summary>
        /// <param name="item">列表条目对象</param>
        /// <param name="parentFolderId">父文件夹ID</param>
        /// <param name="libRootUrl">文档库根路径</param>
        public ServerItem(ListItem item, int parentFolderId, string libRootUrl)
            : this(item)
        {
            this.LibRelativeUrl = this.ServerRelativeUrl.Substring(libRootUrl.Length + 1);
            this.ParentFolderId = parentFolderId;
        }
    }
}
