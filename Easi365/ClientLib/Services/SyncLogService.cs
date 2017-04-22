using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientLib.SyncLogService;
using ClientLib.Entities;

namespace ClientLib.Services
{
    public class SyncLogService
    {
        private readonly string ServiceUrl = ":10240/Service/LoggingService.asmx";
        public SyncLogService()
        {
        }

        public void SyncLog(IEnumerable<Log> logs)
        {
            string logSerUrl = Core.CoreManager.ConfigManager.Settings.ServerUrl + ServiceUrl;
            
            LoggingService ser = new LoggingService();
            ser.Url = logSerUrl;
            ser.Timeout = 5000;
            ser.AddLog(logs.ToArray());
        }
    }
}
