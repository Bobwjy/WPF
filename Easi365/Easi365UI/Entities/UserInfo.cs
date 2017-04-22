using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easi365UI.Entities
{
    public class UserInfo
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 是否记住密码
        /// </summary>
        public bool IsRememberPwd { get; set; }
        /// <summary>
        /// 是否自动登录
        /// </summary>
        public bool IsAutoLogin { get; set; }
    }
}
