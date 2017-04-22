using ClientLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easi365UI.Models.SkyDrive
{
    public class SkyDriveItemModel : DiskEntryModelBase
    {

        public int Version { get; set; }
        public DateTime Modified { get; set; }

        public ServerItem ServerItem { get; set; }
        public int Size { get; set; }

        public int SharedWith { get; set; }

        public SkyDriveItemModel()
        {

        }

        public SkyDriveItemModel(IProfile profile)
            : base(profile)
        {
        }

        public SkyDriveItemModel(IProfile profile, ServerItem si)
            : base(profile)
        {
            ServerItem = si;

            this.IsDirectory = si.ItemType == FileOrFolderType.Folder;
            this.Name = si.Name;
            //this.Parent = new SkyDriveItemModel(profile, si.ParentFolder);
           // this.IsRenamable = true;
            this.FullPath = si.ServerRelativeUrl;

            this.Version = si.Version;
            this.Modified = si.Modified;

            this.Size = si.FileSize;
            this.SharedWith = si.SharedWith;
        }

        public static SkyDriveItemModel DefaultSkyDriveItemModel(SkyDriveProfile profile, string rootPath)
        {
            return new SkyDriveItemModel(profile) { FullPath = rootPath };
        }
    }
}
