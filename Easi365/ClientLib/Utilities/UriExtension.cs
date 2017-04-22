using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientLib.Utilities
{
    public static class UriExtension
    {
        /// <summary>
        /// 根据文件的路径 获取文件所在的web路径
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetWebUriByFile(this Uri uri)
        {
            // 网络文件的地址格式
            // 如果是文件的话 使用segments拼接url 数组的最后一位是文件 倒数第二位是文档库(或者是文件夹) 
            // 剩下的地址为网站地址?
            //https://sinoserve-my.sharepoint.cn:443/personal/wangxiaodong_sinoserve_com/Documents/0140417160602.png"
            StringBuilder uriBuilder = new StringBuilder();
            uriBuilder.Append(uri.Scheme);
            uriBuilder.Append("://");
            uriBuilder.Append(uri.Host);

            if (uri.LocalPath.IndexOf("personal") > -1)
            {
                for (int i = 0; i < 3; i++)
                    uriBuilder.Append(uri.Segments[i]);
            }
            else
            {
                for (int i = 0; i < uri.Segments.Length - 2; i++)
                    uriBuilder.Append(uri.Segments[i]);
            }

            return uriBuilder.ToString();
        }

        public static string GetWebHost(this Uri uri)
        {
            StringBuilder uriBuilder = new StringBuilder();
            uriBuilder.Append(uri.Scheme);
            uriBuilder.Append("://");
            uriBuilder.Append(uri.Host);

            return uriBuilder.ToString();
        }
    }
}
