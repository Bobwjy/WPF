using ClientLib.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ClientLib.Entities
{
    public class ClientItem
    {
        /// <summary>
        /// 文件/文件夹基本属性
        /// </summary>
        public FileSystemInfo BasicInfo { get; set; }
        /// <summary>
        /// 文件在磁盘分区上的唯一标识（NTFS上唯一，FAT分区可能会改变）
        /// </summary>
        public string FileIndex { get; private set; }
        /// <summary>
        /// NTFS分区的扩展ID
        /// </summary>
        public string ADSId { get; private set; }

        public FileOrFolderType ItemType 
        {
            get
            {
                return (this.BasicInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory ?
                    FileOrFolderType.Folder : FileOrFolderType.File;
            }
        }
        private string _hash;
        /// <summary>
        /// 文件正文的散列值（MD5）
        /// </summary>
        public string Hash
        {
            get
            {
                if (this.ItemType == FileOrFolderType.Folder)
                    return null;
                if (string.IsNullOrEmpty(_hash))
                    _hash = ComputeHash();
                return _hash;
            }
        }

        private string _pathHash;

        /// <summary>
        /// 文件路径的散列值（MD5）
        /// </summary>
        public string PathHash
        {
            get
            {
                if (string.IsNullOrEmpty(_pathHash))
                    _pathHash = ComputePathHash();
                return _pathHash;
            }
        }

        public long Size { get; set; }

        /// <summary>
        /// 父文件夹的FileIndex
        /// </summary>
        public string ParentFolderIndex { get; set; }
        public bool IsParentFolderSet { get; set; }


        public ClientItem(FileSystemInfo fsInfo)
        {
            this.BasicInfo = fsInfo;
            this.FileIndex = GetFileIndex().ToString();

            this.ADSId = GetADSId();
            this.IsParentFolderSet = false;
        }

        public ClientItem(FileSystemInfo fsInfo, string parentFolderIndex)
            : this(fsInfo)
        {
            this.ParentFolderIndex = parentFolderIndex;
            this.IsParentFolderSet = true;
        }

        /// <summary>
        /// 获取文件/文件夹在磁盘分区上的唯一标识
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="lockFile">是否锁定文件（当切换文件夹的时候，通过设置此参数为true，来检查该文件/文件夹是否被其他应用占用</param>
        /// <returns>唯一标识</returns>
        public static ulong GetFileIndex(string path, bool lockFile)
        {
            WinAPI.BY_HANDLE_FILE_INFORMATION objectFileInfo = new WinAPI.BY_HANDLE_FILE_INFORMATION();
            IntPtr fHandle = IntPtr.Zero;
            try
            {
                fHandle = WinAPI.CreateFile(path,
                     WinAPI.GENERIC_READ,
                     lockFile ? 0 : WinAPI.FILE_SHARE_READ | WinAPI.FILE_SHARE_WRITE,
                     IntPtr.Zero,
                     WinAPI.OPEN_EXISTING,
                     WinAPI.FILE_FLAG_BACKUP_SEMANTICS,
                     IntPtr.Zero);
                if (fHandle.ToInt32() == -1)
                    return 0;
                WinAPI.GetFileInformationByHandle(fHandle, out objectFileInfo);
                return ((ulong)objectFileInfo.FileIndexHigh << 32) + (ulong)objectFileInfo.FileIndexLow;
            }
            catch
            {
                return 0;
            }
            finally
            {
                WinAPI.CloseHandle(fHandle);
            }
        }

        private ulong GetFileIndex()
        {
            return GetFileIndex(this.BasicInfo.FullName, false);
        }

        /// <summary>
        /// 获取NTFS分区的扩展属性，仅对文件使用（文件夹永远返回null）
        /// </summary>
        /// <returns></returns>
        private string GetADSId()
        {
            if (this.ItemType == FileOrFolderType.Folder)
                return null;
            IntPtr fHandle = IntPtr.Zero;
            try
            {
                fHandle = WinAPI.CreateFile(this.BasicInfo.FullName + ":client_id.prop",
                    WinAPI.GENERIC_READ,
                    WinAPI.FILE_SHARE_READ | WinAPI.FILE_SHARE_WRITE,
                    IntPtr.Zero,
                    WinAPI.OPEN_EXISTING,
                    WinAPI.FILE_FLAG_BACKUP_SEMANTICS,
                    IntPtr.Zero);
                if (fHandle.ToInt32() == -1)
                    return null;

                uint size = WinAPI.GetFileSize(fHandle, IntPtr.Zero);
                byte[] buffer = new byte[size];
                uint read = 0;
                if (!WinAPI.ReadFile(fHandle, buffer, size, ref read, IntPtr.Zero))
                    return null;
                return Encoding.ASCII.GetString(buffer);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (fHandle != IntPtr.Zero && fHandle.ToInt32() != -1)
                    WinAPI.CloseHandle(fHandle);
            }
        }

        /// <summary>
        /// 计算文件MD5
        /// </summary>
        private string ComputeHash()
        {
            try
            {
                MD5 md5 = MD5.Create();
                using (FileStream fs = new FileStream(this.BasicInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return Convert.ToBase64String(md5.ComputeHash(fs));
                }
            }
            catch
            {
                return null;
            }
        }

        private string ComputePathHash()
        {
            try
            {
                MD5 md5 = MD5.Create();
                byte[] pathData = Encoding.UTF8.GetBytes(this.BasicInfo.FullName);
                return Convert.ToBase64String(md5.ComputeHash(pathData));
            }
            catch
            {
                return null;
            }
        }

    }
}
