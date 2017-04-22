using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLib.SyncActions
{
    public enum SyncActionType
    {
        /// <summary>
        /// 创建文件夹
        /// </summary>
        CreateFolder,
        /// <summary>
        /// 上载/下载文件（新建）
        /// </summary>
        CreateFile,
        /// <summary>
        /// 删除文件夹
        /// </summary>
        DeleteFolder,
        /// <summary>
        /// 删除文件
        /// </summary>
        DeleteFile,
        /// <summary>
        /// 更新文件
        /// </summary>
        UpdateFile
    }
}
