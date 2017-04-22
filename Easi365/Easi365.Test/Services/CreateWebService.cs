//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using ClientLib;
//using SP=Microsoft.SharePoint.Client;

//namespace Easi365.Test.Services
//{
//    public class CreateWebService
//    {
//        private ServerSide _server;

//        public CreateWebService(ServerSide server)
//        {
//            _server = server;
//        }

//        public SP.Web Create(string url, string title, string desc)
//        {
//            try
//            {
//                SP.Web web = null;
//                SP.ExceptionHandlingScope exScop = new SP.ExceptionHandlingScope(_server.ClientCtx);
//                using (exScop.StartScope())
//                {
//                    using (exScop.StartTry())
//                    {
//                        var webInfo = new SP.WebCreationInformation();
//                        webInfo.Url = url;
//                        webInfo.Title = title;
//                        webInfo.Description = desc;

//                        web = _server.ClientCtx.Web.Webs.Add(webInfo);
//                        web.Update();
//                        _server.ClientCtx.Load(web,
//                            w => w.Id,
//                            w => w.Title,
//                            w => w.Description);
//                    }
//                    using (exScop.StartCatch())
//                    {
                        
//                    }
//                }
//                _server.ClientCtx.ExecuteQuery();

//                if (exScop.HasException)
//                {
//                    throw new Exception("创建网站失败");
//                }
//                else
//                {
//                    //add created information
//                    return web;
//                }
//            }
//            catch (Exception ex)
//            {
                
//                throw;
//            }
//        }
//    }
//}
