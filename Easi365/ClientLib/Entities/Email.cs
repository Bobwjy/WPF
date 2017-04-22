using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLib.Entities
{
    public class Email
    {
        /// <summary>
        /// 邮件ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 发送人
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public string SendTime { get; set; }
    }
}
