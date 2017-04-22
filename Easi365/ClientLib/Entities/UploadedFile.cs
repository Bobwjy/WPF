using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easi365DB;

namespace ClientLib.Entities
{
    public class UploadedFile
    {
        DB.UploadingFileDetailDataTable _uploadingDetailTable;

        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime CreatedDate { get; set; }

        bool _isDirectory = false;
        public bool IsDirectory
        {
            get
            {
                return _isDirectory;
            }
        }

        public UploadedFile(ServerItem si)
        {
            this.FileName = si.Name;
            this.Size = si.FileSize;
            this.CreatedDate = si.Modified;
        }

        public static UploadedFile GetUploadedFileByServerItem(ServerItem si)
        {
            UploadedFile item = new UploadedFile(si);
            return item;
        }

        public UploadedFile(DB.UploadingFileDetailRow row)
        {
            this.FileName = row.FileName;
            this.Size = row.Size;
            this.CreatedDate = row.CreatedDate;
        }

        public UploadedFile(DB.UploadingFileDetailDataTable uploadingDetailTable)
        {
            _uploadingDetailTable = uploadingDetailTable;
        }

        public List<UploadedFile> UploadedFilesToList()
        {
            List<UploadedFile> uploadedFiles = new List<UploadedFile>();

            foreach (DB.UploadingFileDetailRow row in _uploadingDetailTable.Rows)
                uploadedFiles.Add(new UploadedFile(row));

            return uploadedFiles;
        }
    }
}
