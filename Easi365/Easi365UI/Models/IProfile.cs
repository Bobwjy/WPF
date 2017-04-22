using ClientLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easi365UI.Models
{
    public interface IProfile
    {
        //IComparer<IEntryModel> GetComparer(ColumnInfo column);

        /// <summary>
        /// Return the entry that represent the path, or null if not exists.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        //Task<IEntryModel> ParseAsync(string path);

        /// <summary>
        /// 异步加载所需要的项目
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="fileListItems"></param>
        /// <param name="refresh"></param>
        Task ListAsync(IEntryModel entry, ObservableCollection<IEntryModel> fileListItems, bool refresh = false);
        Task ListSearchAsync(string content, ObservableCollection<IEntryModel> fileListItems, IEntryModel entry, bool refresh = false); 

        ServerSide Server { get; }
    }
}
