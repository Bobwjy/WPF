using ClientLib.Entities;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ClientLib.Utilities;

namespace ClientLib.Services
{
    public class EmailService
    {
        static ExchangeService service;

        public EmailService(string userEmail, string passWord)
        {
            string _passWord = passWord;

            service = new ExchangeService(ExchangeVersion.Exchange2013);
            service.Credentials = new WebCredentials(userEmail, _passWord);
            service.AutodiscoverUrl(userEmail, RedirectionUrlValidationCallback);
        }

        /// <summary>
        /// 获取未读邮件数
        /// </summary>
        public string GetUnreadMailCount() {
            int unRead = 0;
            unRead = Folder.Bind(service, WellKnownFolderName.Inbox).UnreadCount;
            return unRead.ToString();
        }

        /// <summary>
        /// 获取未读邮件
        /// </summary>
        public List<Email> GetUnreadMailFromInbox()
        {
            ItemView view = new ItemView(int.MaxValue);
            FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, SetFilter(), view);
            ServiceResponseCollection<GetItemResponse> items =
                service.BindToItems(findResults.Select(item => item.Id), new PropertySet(BasePropertySet.FirstClassProperties, EmailMessageSchema.From, EmailMessageSchema.ToRecipients));

            return items.Select(item =>
            {
                return new Email()
                {
                    ID = item.Item.Id.ToString(),
                    From = ((EmailAddress)item.Item[EmailMessageSchema.From]).Name,
                    Subject = GetStr(item.Item.Subject,32),
                    Body = GetStr(ClientLib.Utilities.Common.NoHTML(item.Item.Body.ToString()), 50),
                    SendTime = item.Item.DateTimeSent.ToShortDateString(),
                };
            }).ToList();
        }

        /// <summary>
        /// 设置获取什么类型的邮件
        /// </summary>
        private static SearchFilter SetFilter()
        {
            List<SearchFilter> searchFilterCollection = new List<SearchFilter>();
            searchFilterCollection.Add(new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false));
            SearchFilter s = new SearchFilter.SearchFilterCollection(LogicalOperator.And, searchFilterCollection.ToArray());

            return s;
        }

        /// <summary>
        /// 重定向URL验证回调
        /// </summary>
        static bool RedirectionUrlValidationCallback(String redirectionUrl)
        {
            bool redirectionValidated = false;
            //https://autodiscover-s.outlook.com/autodiscover/autodiscover.xml
            if (redirectionUrl.Equals("https://autodiscover-s.partner.outlook.cn/autodiscover/autodiscover.xml"))
                redirectionValidated = true;

            return redirectionValidated;
        }

        /// <summary>        
        /// 截取字符串        
        /// </summary>        
        /// <param name="s">源字符串</param>        
        /// <param name="num">截取字符数（中文字符等于两个英文字符）</param>        
        /// <returns>截取后的字符串</returns>        
        public static string GetStr(string s, int num)
        {
            int l = num;
            string temp = s;
            if (Regex.Replace(temp, "[^\x00-\x80]", "zz", RegexOptions.IgnoreCase).Length <= l)
            {
                return temp;
            }
            for (int i = temp.Length; i >= 0; i--)
            {
                temp = temp.Substring(0, i);
                if (Regex.Replace(temp, "[^\x00-\x80]", "zz", RegexOptions.IgnoreCase).Length <= l - 3)
                    return temp + "...";
            }

            return "";
        }

    }
}
