using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ClientLib.Core
{
    [Serializable]
    public class Easi365Settings
    {
        public string ServerMode = "Local";//Local Office365
        public string ServerUrl = "http://10.135.22.183";  //服务器地址

        public string Office365ServerUrl = "https://easi365.sharepoint.com";  //Office365服务器地址
        public string LocalServerUrl = "http://10.135.22.183";  //本地服务器地址10.135.22.183 192.168.0.118

        public bool Logging = true;//是否记录日志
        public string DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                 "Easi365" + Path.DirectorySeparatorChar + "DB");//本地数据库路径

        public string FilesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                 "Easi365" + Path.DirectorySeparatorChar + "Files");//本地文件

        public string PersonalFilesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                 "Easi365" + Path.DirectorySeparatorChar + "Files" + Path.DirectorySeparatorChar + "PersonalFolder");

        public string PersonalFilesCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                 "Easi365" + Path.DirectorySeparatorChar + "Files" + Path.DirectorySeparatorChar + "PersonalFolder"+
                 Path.DirectorySeparatorChar + "Cache" + Path.DirectorySeparatorChar);

        public string CompanyFilesCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                 "Easi365" + Path.DirectorySeparatorChar + "Files" + Path.DirectorySeparatorChar + "CompanyFolder" +
                 Path.DirectorySeparatorChar + "Cache" + Path.DirectorySeparatorChar);

        public string LocalFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                 "Easi365" + Path.DirectorySeparatorChar + "Local");//本地文件夹路径

        public string ID = "";  //ID
        public string CurrentUserName = "";  //当前用户名
        public string PassWord = "";  //密码

        public double AutoSaveFileToServerTick = 5000;//默认5秒更新一次上传任务文件（客户端保存文件后自动更新到服务器端）

        public string ViewMode = "Details";  //默认文件视图呈现方式

        public string CollapseMode = "Collapse";  //默认展开侧边栏

        public string MThumbTimeStamp = "";
    }
}
