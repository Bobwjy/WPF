using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace ClientLib.Utilities
{
    /// <summary>
    /// WINAPI调用，用于文件操作（获取分区标识符和ADS扩展属性）
    /// </summary>
    public class WinAPI
    {
        #region Win32 Constants
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint GENERIC_READ = 0x80000000;

        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;

        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;
        public const uint OPEN_ALWAYS = 4;

        public const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
        #endregion

        public struct BY_HANDLE_FILE_INFORMATION
        {
            public uint FileAttributes;
            public ComTypes.FILETIME CreationTime;
            public ComTypes.FILETIME LastAccessTime;
            public ComTypes.FILETIME LastWriteTime;
            public uint VolumeSerialNumber;
            public uint FileSizeHigh;
            public uint FileSizeLow;
            public uint NumberOfLinks;
            public uint FileIndexHigh;
            public uint FileIndexLow;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetFileInformationByHandle(IntPtr hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(string filename, uint desiredAccess, uint sharedMode, IntPtr securityAttributes,
                                            uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32", SetLastError = true)]
        public static extern uint GetFileSize(IntPtr handle, IntPtr size);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool ReadFile(IntPtr handle, byte[] buffer, uint byteToRead, ref uint bytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite,
            ref uint lpNumberOfBytesWritten, IntPtr lpOverlapped);
    }
}
