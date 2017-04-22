using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Easi365UI.Util
{
    public class NetWorkHelper
    {
        public bool CheckNetWork(string pingPath)
        {
            bool netWorkStatus = false;
            Ping pingSender = new Ping();
            PingReply reply = null;
            try
            {
                //string JsServerWebURLKey = appSettingsHelper.GetValue(Constants.JsServerWebURLKey);
                //int jsServerPoint = JsServerWebURLKey.LastIndexOf(":");
                //string pingPath = JsServerWebURLKey.Substring(7);
                //if (jsServerPoint > 7)
                //{
                //    pingPath = JsServerWebURLKey.Substring(7, jsServerPoint - 7);
                //}

                reply = pingSender.Send(pingPath, 1000);
            }
            catch (Exception ex)
            {
               //add log
            }
            finally
            {
                if (reply == null || (reply != null && reply.Status != IPStatus.Success))
                {
                    netWorkStatus = false;
                }
                else if (reply.Status == IPStatus.Success)
                {
                    netWorkStatus = true;
                }

            }
            return netWorkStatus;
        }
    }
}
