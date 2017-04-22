using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.SharePoint.Client.Utilities;
using ClientLib.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ClientLib.Utilities
{
    public static class ClientContextExtension
    {
        public async static void SaveFileDirectWithoutOverwrite(ClientContext ctx, string serverRelUrl, Stream stream, UploadState uploadState, Action<bool> uploaded = null)
        {
            string absolutePath = (new Uri(new Uri(ctx.Url), serverRelUrl)).AbsoluteUri;

            WebRequestExecutor webRequestExecutor = ctx.WebRequestExecutorFactory.CreateWebRequestExecutor(ctx, absolutePath);
            webRequestExecutor.RequestKeepAlive = false;
            webRequestExecutor.RequestMethod = "PUT";
            webRequestExecutor.RequestHeaders[HttpRequestHeader.IfNoneMatch] = "*";

            webRequestExecutor.WebRequest.AllowAutoRedirect = false;
            webRequestExecutor.WebRequest.AllowWriteStreamBuffering = false;

            Stream requestStream = webRequestExecutor.GetRequestStream();
            byte[] numArray = new byte[0x400];

            while (true)
            {
                int num = await stream.ReadAsync(numArray, 0, 0x400);

                int num1 = num;
                if (num <= 0)
                {
                    break;
                }
                //await requestStream.WriteAsync(numArray, 0, num1);

                //uploadState.BytesWrite += num;
                //uploadState.UploadPercent = ((double)uploadState.BytesWrite / (double)uploadState.TotalBytes);
            }
            requestStream.Flush();
            requestStream.Close();

            //var method = typeof(ClientContext).GetMethod("FireExecutingWebRequestEventInternal",
            //    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.ExactBinding);
            //method.Invoke(ctx, new object[] { new WebRequestEventArgs(webRequestExecutor) });

            try
            {
                bool hasError = false;
                webRequestExecutor.Execute();
                if (webRequestExecutor.StatusCode != HttpStatusCode.Created && webRequestExecutor.StatusCode != HttpStatusCode.OK)
                {
                    hasError = true;
                    object[] responseContentType = new object[2];
                    responseContentType[0] = webRequestExecutor.ResponseContentType;
                    responseContentType[1] = webRequestExecutor.StatusCode;
                    throw new ClientRequestException(Resources.GetString("RequestUnexpectedResponse", responseContentType));
                }
                //上传完成的回调函数
                if (uploaded != null)
                    uploaded(hasError);
            }
            catch (WebException webEx)
            {
                WebException webException = webEx;
                HttpWebResponse response = webException.Response as HttpWebResponse;
                if (response == null || response.StatusCode != HttpStatusCode.PreconditionFailed)
                {
                    throw;
                }
                else if (response.Headers["X-MSDAVEXT_Error"] != null)
                {
                    if (response.Headers["X-MSDAVEXT_Error"].StartsWith("589923;"))
                        throw new Exceptions.ServerFullException();
                    else if (response.Headers["X-MSDAVEXT_Error"].StartsWith("589951;"))
                        throw new ClientRequestException(Resources.GetString("FileAlreadyExists"));
                    else
                    {
                        string errorInfo = response.Headers["X-MSDAVEXT_Error"];
                        string code = errorInfo.Substring(0, errorInfo.IndexOf(';'));
                        string text = errorInfo.Substring(errorInfo.IndexOf(';') + 1);
                        throw new Exceptions.UploadException(string.Format("{0}({1})",
                            HttpUtility.UrlKeyValueDecode(text), code));
                    }
                }
                else
                {
                    throw new ClientRequestException(Resources.GetString("FileAlreadyExists"));
                }
            }
        }

        public static void ClearPendingExecution(ClientContext ctx)
        {
            if (ctx.HasPendingRequest)
            {
                try
                {
                    ctx.ExecuteQuery();
                }
                catch
                {
                }
            }
        }



        public static async Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request, CancellationToken ct)
        {
            using (ct.Register(() => {
                try
                {
                    request.Abort();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    request = null;
                    GC.Collect();
                }
            }, useSynchronizationContext: false))
            {
                try
                {
                    var response = await request.GetResponseAsync();
                    ct.ThrowIfCancellationRequested();
                    return (HttpWebResponse)response;
                }
                catch (WebException ex)
                {
                    // WebException is thrown when request.Abort() is called,
                    // but there may be many other reasons,
                    // propagate the WebException to the caller correctly
                    if (ct.IsCancellationRequested)
                    {
                        // the WebException will be available as Exception.InnerException
                        throw new OperationCanceledException(ex.Message, ex, ct);
                    }
                    // cancellation hasn't been requested, rethrow the original WebException
                    throw;
                }
            }
        }

    }
}
