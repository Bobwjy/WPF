using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Easi365DB;

namespace ClientLib.Entities
{
    public class LocalFilesDict
    {
        /// <summary>
        /// 根据磁盘分区标识符查找文件信息
        /// </summary>
        public Dictionary<string, DB.FileInfoRow> FileIndexDict { get; private set; }

        /// <summary>
        /// 根据服务器端Id查找文件信息
        /// </summary>
        public Dictionary<int, DB.FileInfoRow> ServerIdDict { get; private set; }

        public Dictionary<string, DB.FileInfoRow> FilePathHashDict { get; private set; }

        /// <summary>
        /// 根据服务器端UniqueId查找文件信息
        /// </summary>
        public Dictionary<string, DB.FileInfoRow> ServerUniqueId { get; private set; }

        public LocalFilesDict(DB.FileInfoDataTable fileInfoTable)
        {
            FileIndexDict = new Dictionary<string, DB.FileInfoRow>();
            ServerIdDict = new Dictionary<int, DB.FileInfoRow>();
            ServerUniqueId = new Dictionary<string, DB.FileInfoRow>();

           // FilePathHashDict = new Dictionary<string, DB.FileInfoRow>();

            foreach (DB.FileInfoRow infoRow in fileInfoTable.Rows)
            {
                this.FileIndexDict.Add(infoRow.FileIndex, infoRow);

                if (!infoRow.IsServerIndexNull() && infoRow.ServerIndex > 0)
                {
                    if (!this.ServerIdDict.ContainsKey(infoRow.ServerIndex))
                        this.ServerIdDict.Add(infoRow.ServerIndex, infoRow);
                }
                if (!infoRow.IsUidNull())
                {
                    if (!this.ServerUniqueId.ContainsKey(infoRow.Uid))
                        this.ServerUniqueId.Add(infoRow.Uid, infoRow);
                }
                //if (!infoRow.IsPathHashNull())
                //{
                //    if (!this.FilePathHashDict.ContainsKey(infoRow.PathHash))
                //        this.FilePathHashDict.Add(infoRow.PathHash, infoRow);
                //}
            }
        }
    }
}
