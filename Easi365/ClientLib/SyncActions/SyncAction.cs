using ClientLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLib.SyncActions
{
    public abstract class SyncAction
    {
        /// <summary>
        /// 操作目的端
        /// </summary>
        public abstract SyncTargetType TargetType { get; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public abstract SyncActionType SyncType { get; }
        /// <summary>
        /// 服务器端条目信息
        /// </summary>
        public abstract ServerItem ServerItem { get; set; }
        /// <summary>
        /// 客户端条目信息
        /// </summary>
        public abstract ClientItem ClientItem { get; set; }
        /// <summary>
        /// 服务器端Id
        /// </summary>
        public abstract int ServerId { get; }
        /// <summary>
        /// 客户端Id
        /// </summary>
        public abstract string ClientId { get; }
        /// <summary>
        /// 目标路径
        /// </summary>
        public abstract string Path { get; }
        /// <summary>
        /// 原路径（用于移动或重命名）
        /// </summary>
        public abstract string OldPath { get; }
        /// <summary>
        /// 执行操作
        /// </summary>
        public abstract void Execute(ServerSide server, bool autoResolveConflict, Action<bool> complete);

        /// <summary>
        /// 操作的文字描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}{1} | {2} - {3}",
                this.TargetType, this.SyncType, this.OldPath, this.Path);
        }

        /// <summary>
        /// 忽略操作
        /// </summary>
        private bool _ignore = false;
        public bool Ignore
        {
            get { return _ignore; }
            set { _ignore = value; }
        }
    }
}
