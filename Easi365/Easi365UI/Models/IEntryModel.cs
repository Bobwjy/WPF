using ClientLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easi365UI.Models
{
    public interface IEntryModel
    {
        IProfile Profile { get; }
        bool IsDirectory { get; } //是否为目录 文件/文件夹
        IEntryModel Parent { get; }
        //string Label { get; }
        string Name { get; } // 文件名称
        string Description { get; } //
        string FullPath { get; } //完整路径
       // bool IsRenamable { get; }
        DownloadState DownloadState { get;}
        //bool IsDownloading { get; }
    }
}
