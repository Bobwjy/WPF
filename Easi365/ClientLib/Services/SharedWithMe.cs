using ClientLib.Core;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLib.Entities;
using System.Web.Script.Serialization;

namespace ClientLib.Services
{
    public class SharedWithMe
    {
        public int LastRow = -1;
        public int TotalRows = 0;
        public List<ServerItem> Row { get; set; }
    }

    public class SharedWithMeContext
    {
        public static SharedWithMeContext DocumentsSharedWithMeContext = null;

        ClientContext _clientContext;
        ClientRuntimeContext _runtimeContext;
        private SharedWithMeContext(ClientContext clientContext)
        {
            _clientContext = clientContext;
        }

        public static SharedWithMeContext GetInstence(ClientContext clientContext)
        {
            if (DocumentsSharedWithMeContext == null)
            {
                DocumentsSharedWithMeContext = new SharedWithMeContext(clientContext);
            }
            return DocumentsSharedWithMeContext;
        }

        const string TYPEID = "{1118ef92-5f52-4de7-853f-edf3f1229990}";
        const string METHODNAME = "GetListDataScript";
        const string WEBPARTNAME = "WPQ5";
        const string SORTFIELD = "Modified";
        const bool ASCENDINGSORT = false;
        const int PAGESIZE = 30;
        const bool ISEXPFEATUREDLIENTENABLED = false;

        int LastRow = -1;
        int TotalRows = 0;
        public List<ServerItem> AllRows { get; set; }
        
        /// <summary>
        /// 分页读取共享给我的数据
        /// </summary>
        /// <param name="pageIndex">当前页（从0开始）</param>
        public void GetSharedDocumentsWithMeByPage(int pageIndex)
        {
            if (pageIndex == 0)
            {
                this.LastRow = -1;
                this.TotalRows = 0;
                this.AllRows = new List<ServerItem>();
            }
            if (this.LastRow == this.TotalRows) return;

            try
            {
                //初始 ClientRuntimeContext
                if (_runtimeContext == null)
                {
                    var web = _clientContext.Web;
                    _clientContext.Load(web);
                    _clientContext.ExecuteQuery();
                    _runtimeContext = web.Context;
                }

                //计算开始行
                int startRow = 0;
                if (pageIndex >= 0)
                {
                    startRow = pageIndex * PAGESIZE;
                }

                //读取数据脚本
                var result = new ClientActionInvokeStaticMethod(
                    context: _runtimeContext,
                    typeId: TYPEID,
                    methodName: METHODNAME,
                    parameters: new object[] 
                    { 
                        WEBPARTNAME, 
                        SORTFIELD, 
                        ASCENDINGSORT, 
                        startRow, 
                        PAGESIZE
                    });
                _runtimeContext.AddQuery(result);
                ClientResult<string> clientResult = new ClientResult<string>();
                _runtimeContext.AddQueryIdAndResultObject(result.Id, clientResult);
                _runtimeContext.ExecuteQuery();

                //解析返回的数据脚本
                this.ResolveData(clientResult.Value);

                //如果未读取完继续读取
                if (this.LastRow != this.TotalRows && this.LastRow >= 0)
                { 
                    this.GetSharedDocumentsWithMeByPage(++pageIndex);
                }
            }
            catch (Exception ex)
            {
                Logging.Add("SharedWithMe - 读取数据失败", ex);
            }
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="data"></param>
        void ResolveData(string data)
        {
            try
            {
                int start = data.IndexOf("sharedWithMeViewCtx.ListData") + "sharedWithMeViewCtx.ListData".Length;
                int end = data.LastIndexOf("sharedWithMeViewCtx.ListSchema.UserDispParam");
                data = data.Substring(start + 3, end - start - 5);
                JavaScriptSerializer ser = new JavaScriptSerializer();

                var docs = ser.Deserialize<SharedWithMe>(data);
                this.LastRow = docs.LastRow;
                this.TotalRows = docs.TotalRows;
                this.AllRows.AddRange(docs.Row);
            }
            catch (Exception ex)
            {
                Logging.Add("SharedWithMe - 异常或没有数据", ex);
            }
        }
    }
}
