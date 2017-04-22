//using Microsoft.SharePoint.Client;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security;
//using System.Text;
//using Easi365.Test.Services;
//using IO = System.IO;
//using System.Net;
//using System.IO;

//using ClientLib;
//using ClientLib.Core;

//namespace Easi365.Test
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            //string uploadpath = "https://easi365-my.sharepoint.com/personal/zhoutao_easi365_onmicrosoft_com/Documents/easi365/%E8%B6%85%E5%94%AF%E7%BE%8E%E9%92%A2%E7%90%B4%E6%9B%B2%E9%93%83%E5%A3%B0-Children%20Of%20The%20Earth.mp3";
//            ////string serverUrl = "https://servinbus-my.sharepoint.com/personal/wangxiaodong_sinoserve_com";
//            ////string database = @"D:\工作\公司\数据库文件\Easy365DB.mdb";
//            ////wangxiaodong@easi365.onmicrosoft.com  Kaco3410
//            //string serverUrl = "https://easi365-my.sharepoint.com/personal/zhoutao_easi365_onmicrosoft_com";
//            ////string serverUrl = "https://easi365.sharepoint.com";
//            //string database = @"D:\工作\公司\数据库文件\Easy365DB.mdb";

//            //SyncManager sm = new SyncManager(serverUrl, database, "zhoutao@easi365.onmicrosoft.com", "pass@word1");
//            //sm.Init();

//            //初始化核心
//            //CoreManager.Initialize(
//            //     Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
//            //     "Easi365" + Path.DirectorySeparatorChar));

//           var credential = new SharePointOnlineCredentials("zhoutao@easi365.onmicrosoft.com", GenSecurityPassword("pass@word1"));

//            WebClient downLoader = new WebClient();
//            downLoader.Credentials = credential;
//            //downLoader.Credentials = new NetworkCredential("zhoutao@easi365.onmicrosoft.com", "pass@word1");
//            downLoader.Headers["Accept"] = "application/x-ms-application, image/jpeg, application/xaml+xml, image/gif, image/pjpeg, application/x-ms-xbap, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
//            downLoader.Headers["User-Agent"] = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; MDDC)";
//            downLoader.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");

//            var data = downLoader.DownloadData("https://easi365-my.sharepoint.com/personal/zhoutao_easi365_onmicrosoft_com/Easi365ClientDoc/917f0bc4daf7fee0ed147164ae031f31%20-%20%E5%89%AF%E6%9C%AC.jpg");

//            System.IO.File.WriteAllBytes(@"C:\Users\xiaodong\AppData\Roaming\Easi365\Files\testasdfasdfasd.jpg", data);

//            downLoader.DownloadFile("https://easi365-my.sharepoint.com/personal/zhoutao_easi365_onmicrosoft_com/Easi365ClientDoc/917f0bc4daf7fee0ed147164ae031f31%20-%20%E5%89%AF%E6%9C%AC.jpg",
//                @"C:\Users\xiaodong\AppData\Roaming\Easi365\Files\test.jpg");

//            Console.WriteLine("....");
//            Console.Read();
//        }


//        private static SecureString GenSecurityPassword(string password)
//        {
//            if (string.IsNullOrEmpty(password))
//                throw new ArgumentException("Password");

//            SecureString pwd = new SecureString();
//            foreach (char c in password)
//                pwd.AppendChar(c);

//            return pwd;
//        }

//        private static void ConnectionTest()
//        {
//            using (ClientContext clientContext = new ClientContext("https://servinbus-my.sharepoint.com/personal/wangxiaodong_sinoserve_com"))
//            {
//                string userName = "wangxiaodong@sinoserve.com";
//                SecureString passWord = new SecureString();
//                foreach (char c in "capad1946!")
//                    passWord.AppendChar(c);

//                clientContext.Credentials = new SharePointOnlineCredentials(userName, passWord);
//                Web web = clientContext.Web;

//                List doc = clientContext.Web.Lists.GetByTitle("Documents");

//                clientContext.Load(doc, lib => lib.Id, lib => lib.RootFolder);
//                clientContext.Load(doc.RootFolder,
//                    folder => folder.ServerRelativeUrl,
//                    folder => folder.Folders);
//                clientContext.ExecuteQuery();

//                //var folders = doc.RootFolder.Folders;
//                foreach (Folder folder in doc.RootFolder.Folders)
//                {
//                    Console.WriteLine(folder.Name);
//                }

//                string rootFolderServerRelative = doc.RootFolder.ServerRelativeUrl;

//                //ListItem folderItm = null;
//                //ExceptionHandlingScope exScop = new ExceptionHandlingScope(clientContext);



//                //        var creationInfo = new ListItemCreationInformation();
//                //        creationInfo.UnderlyingObjectType = FileSystemObjectType.Folder;
//                //        creationInfo.FolderUrl = rootFolderServerRelative+"/" + "Easi365aldfj";
//                //        creationInfo.LeafName = "Easi365aldfj";

//                //        folderItm = doc.AddItem(creationInfo);
//                //        folderItm.Update();
//                //        doc.Update();

//                //        clientContext.Load(folderItm,
//                //            itm => itm.Id,
//                //            itm => itm.FileSystemObjectType,
//                //            itm => itm["FileLeafRef"]);
               

//                //LocalDB db = new LocalDB(@"D:\工作\公司\数据库文件\Easy365DB.mdb");
//                // #region 记录本地文件信息到数据库
//                // //ClientItem ci = new ClientItem(new FileInfo(@"D:\工作\公司\数据库文件\传真浏览客户端.docx"));

//                // //db.AddClientItem(ci, null);
//                // #endregion

//                //db.AddLogMessage("开始同步服务器文件....");


//                //DateTime utcTime = DateTime.UtcNow;
//                //DateTime localTime = DateTime.Now;

//                //Console.WriteLine("utc time is :" + utcTime.AddHours(8).ToString());
//                //Console.WriteLine("local time is :" + localTime.ToString());
//                Console.WriteLine("...");
//                Console.Read();
//            }
//        }
//    }
//}
