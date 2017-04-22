using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLib.SyncActions
{
    /// <summary>
    /// 同步操作目的
    /// </summary>
    public enum SyncTargetType
    {
        /// <summary>
        /// 针对客户端进行操作
        /// </summary>
        Client,
        /// <summary>
        /// 针对服务器端进行操作
        /// </summary>
        Server
    }
}
