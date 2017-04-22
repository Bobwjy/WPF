using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Controls;

namespace Easi365UI.Extention
{
    public static class WebBrowserExtension
    {
        /// <summary>
        /// 可以屏蔽浏览器控件访问页面时抛出的脚本错误
        /// 如果hide参数传递true，则屏蔽脚本错误
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <param name="hide"></param>
        public static void SuppressScriptErrors(this WebBrowser webBrowser, bool hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) 
                return;

            object objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
            if (objComWebBrowser == null) 
                return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }
    }
}
