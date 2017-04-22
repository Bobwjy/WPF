using ClientLib.Core;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using SP = Microsoft.SharePoint.Client;
using ClientLib.Entities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ClientLib.Utilities;

namespace ClientLib.Services
{
    public class CreateWebService
    {
        private ServerSide _server;
        private SP.ClientContext _context;

        public CreateWebService(ServerSide server)
        {
            _server = server;
            _context = server.ClientCtx;
        }
        public CreateWebService(SP.ClientContext context)
        {
            _context = context;
        }

        public SP.Web Create(string url, string title, string desc)
        {
            try
            {
                SP.Web web = null;
                var webInfo = new SP.WebCreationInformation();
                webInfo.Url = url;
                webInfo.Title = title;
                webInfo.Description = desc;
                webInfo.Language = 2052;
                webInfo.WebTemplate = "STS#0";

                web = _context.Web.Webs.Add(webInfo);
                _context.Load(web,
                    w => w.Id,
                    w => w.Title,
                    w => w.Description);
                _context.ExecuteQuery();

                return web;
            }
            catch (Exception ex)
            {
                Logging.Add("创建空间失败，地址:" + url,ex);
                throw ex;
            }
        }

        public ClientContext UpdateWeb(string webUrl, string title, string description)
        {
            try
            {
                ClientContext context = new ClientContext(webUrl);
                context.Credentials = _context.Credentials;
                Web web = context.Web;
                web.Title = title;
                web.Description = description;
                web.Update();
                context.ExecuteQuery();

                return context;
            }
            catch(Exception ex) {
                Logging.Add("更新协作空间信息报错", ex);
                return null;
            }
        }

        public void DeleteWeb(string webUrl)
        {
            try
            {
                ClientContext context = new ClientContext(webUrl);
                context.Credentials = _context.Credentials;
                Web web = context.Web;
                web.DeleteObject();
                web.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                Logging.Add("删除协作空间信息报错", ex);
            }
        }

        public void AddPremissions(SP.Web web, SP.Principal principal, SP.RoleType role)
        {
            try
            {
                //RoleDefinitionBindingCollection roleDefinitionBinding = new RoleDefinitionBindingCollection(_context);
                //roleDefinitionBinding.Add(_context.Web.RoleDefinitions.GetByType(role));
                //web.RoleAssignments.Add(principal, roleDefinitionBinding);
                //_context.ExecuteQuery();

                var roleDef = new RoleDefinitionBindingCollection(web.Context);
                roleDef.Add(web.RoleDefinitions.GetByType(role));
                web.RoleAssignments.Add(principal, roleDef);
                web.Context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                Logging.Add("网站添加权限失败.Web:" + web.Url, ex);
                throw ex;
            }
        }
        public void AddPremissions(SP.Web web, IList<SP.Principal> principals, SP.RoleType role)
        {
            foreach (var principal in principals)
            {
                this.AddPremissions(web, principal, role);
            }
        }

        public void RemovePremissions(SP.Principal principal)
        {
            try
            {
                _context.Web.RoleAssignments.GetByPrincipal(principal).DeleteObject();
                _context.ExecuteQuery();
            }
            catch (Exception ex)
            {
                Logging.Add("移除用户权限 - 未找到用户", ex);
            }
        }
        public void RemovePremissions(IList<SP.Principal> principals)
        {
            foreach (var principal in principals)
            {
                this.RemovePremissions(principal);
            }
        }

        private IEnumerable<ListItem> ListItemCollectionToList(ListItemCollection items)
        {
            for (int i = 0; i < items.Count; i++)
                yield return items[i];

          
        }
        
        public Task<ObservableCollection<Spaces>> GetSpacesList(string listTitle)
        {
            return Task<ObservableCollection<Spaces>>.Factory.StartNew(() =>
            {
                try
                {
                    SP.List list = _context.Web.Lists.GetByTitle(listTitle);
                    SP.CamlQuery query = SP.CamlQuery.CreateAllItemsQuery();
                    SP.ListItemCollection items = list.GetItems(query);
                    _context.Load(items);
                    //_context.Load(items, itms => itms.Include(
                    //    itm => itm.Id,
                    //    itm => itm.DisplayName,
                    //    itm => itm["SpaceUri"],
                    //    itm => itm["IsCreated"],
                    //    itm => itm["SpaceMember"],
                    //    itm => itm["SpaceManager"],
                    //    itm => itm["Description"]
                    //));
                    _context.ExecuteQuery();

                    ObservableCollection<Spaces> spacesList = new ObservableCollection<Spaces>();
                    var enumertor = items.GetEnumerator();
                    while(enumertor.MoveNext())
                    {
                        var listItem = enumertor.Current;

                        var id = listItem.Id.ToString();
                        var spaceTitle = Convert.ToString(listItem["Title"]);
                        var spaceUri = Convert.ToString(listItem["SpaceUri"]);
                        var isCreated = Convert.ToBoolean(listItem["IsCreated"]);
                        var spaceMember = new ObservableCollection<CheckedUser>();
                        spaceMember = GetUsersFromFieldUser(listItem, "SpaceMember");
                        var spaceManager = new ObservableCollection<CheckedUser>();
                        spaceManager = GetUsersFromFieldUser(listItem, "SpaceManager");
                        CoreManager.SpaceCategory spaceCategory;

                        string applicant = "", account = "", status = "";;
                        if (Constants.SpaceSite.CooSpaceListTitle == listTitle)
                        {
                            applicant = Convert.ToString(listItem["Applicant"]);
                            account = Convert.ToString(listItem["Account"]);

                            if (isCreated) status = "已创建的协作空间";
                            else status = "新申请的协作空间";

                            spaceCategory = CoreManager.SpaceCategory.CooSpace;
                        }
                        else {
                            if (isCreated) status = "已创建的部门空间";
                            else status = "未创建的部门空间";

                            spaceCategory = CoreManager.SpaceCategory.DeptSpace;
                        }
                        var description = ClientLib.Utilities.Common.NoHTML(Convert.ToString(listItem["Description"]));

                        Spaces s = new Spaces
                        {
                            ID = id,
                            SpaceTitle = spaceTitle,
                            Applicant = applicant,
                            Account = account,
                            SpaceUri = spaceUri,
                            IsCreated = isCreated,
                            SpaceMember = spaceMember,
                            SpaceAdmin = spaceManager,
                            OriginalManager = spaceManager,
                            Desc = description,
                            Status = status,
                            SpaceCategory = spaceCategory
                        };
                        spacesList.Add(s);
                    }

                    return spacesList;
                }
                catch (Exception ex)
                {
                    Logging.Add("加载空间列表报错", ex);
                    return null;
                }
            });
        }

        public ObservableCollection<CheckedUser> GetUsersFromFieldUser(ListItem listItem, string fieldValue)
        {
            var spaceManager = new ObservableCollection<CheckedUser>();
            try
            {
                if (listItem[fieldValue] != null)
                {
                    var userVaues = listItem[fieldValue] as FieldUserValue[];
                    foreach (var userValue in userVaues)
                    {
                        var user = GetStaffByUserId(userValue.LookupId);
                        if (user != null)
                            spaceManager.Add(new CheckedUser { Id = user.ID, Account = user.Account, UserName = user.UserName });
                    }
                }
                return spaceManager;
            }
            catch { return null; }
        }

        public Task<Spaces> GetCooSpaceInfoById(int id) {
            return Task<Spaces>.Factory.StartNew(() =>
            {
                try
                {
                    Spaces space = new Spaces();
                    SP.List list = _context.Web.Lists.GetByTitle(Constants.SpaceSite.CooSpaceListTitle);
                    SP.ListItem item = list.GetItemById(id);
                    _context.Load(item);
                    _context.ExecuteQuery();
                    if (item != null)
                    {
                        space.SpaceTitle = Convert.ToString(item["Title"]);
                        space.SpaceUri = Convert.ToString(item["SpaceUri"]);
                        space.Desc = ClientLib.Utilities.Common.NoHTML(Convert.ToString(item["Description"]));
                        space.SpaceAdmin = GetUsersFromFieldUser(item, "SpaceManager");
                        space.SpaceMember = GetUsersFromFieldUser(item, "SpaceMember");
                    }

                    return space;
                }
                catch (Exception ex)
                {
                    Logging.Add("加载空间信息报错", ex);
                    return null;
                }
            });
        }

        public Task<bool> DeleteListDataForCooSpace(int id) {
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    SP.List list = _context.Web.Lists.GetByTitle(Constants.SpaceSite.CooSpaceListTitle);
                    SP.ListItem item = list.GetItemById(id);

                    item.DeleteObject();
                    _context.ExecuteQuery();

                    return true;
                }
                catch (Exception ex)
                {
                    Logging.Add("删除空间列表(" + Constants.SpaceSite.CooSpaceListTitle + ")信息报错", ex);
                    return false;
                }
            });

            
        }

        public Staff GetStaffByUserId(int uid)
        {
            Staff staff = null;
            try
            {
                ClientContext clientContext = null;
                clientContext = new ClientContext(CoreManager.ConfigManager.Settings.ServerUrl);
                clientContext.Credentials = _context.Credentials;

                var user = ClientLib.Utilities.Common.GetSiteUserById(clientContext, uid);
                if (user != null)
                {
                    //服务器类型
                    CoreManager.ServerMode mode = (CoreManager.ServerMode)Enum.Parse(typeof(CoreManager.ServerMode), CoreManager.ConfigManager.Settings.ServerMode, true);
                    string account = user.LoginName.GetLoginNameWithoutDomain();
                    if (mode == CoreManager.ServerMode.Office365) {
                        account = account.Replace("i:0#.f|membership|", "");
                    }

                    SP.List staffList = clientContext.Web.Lists.GetByTitle("StaffList");
                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = string.Format("<View><Query><Where><Eq><FieldRef Name='Account'/><Value Type='Text'>{0}</Value></Eq></Where></Query><RowLimit>1</RowLimit></View>", account);
                    ListItemCollection listItems = staffList.GetItems(camlQuery);
                    clientContext.Load(
                         listItems,
                         items => items.Include(
                             item => item.Id,
                             item => item["Title"],
                             item => item["Account"],
                             item => item["TelPhone"],
                             item => item["Dept"]));
                    clientContext.ExecuteQuery();
                    if (listItems != null && listItems.Count > 0)
                    {
                        staff = new Staff();
                        var item = listItems[0];
                        staff.ID = item.Id;
                        staff.UserName = Convert.ToString(item["Title"]);
                        staff.Account = Convert.ToString(item["Account"]);
                        staff.TelPhone = Convert.ToString(item["TelPhone"]);
                        staff.Dept = Convert.ToString(item["Dept"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Add("获取人员报错", ex);
                return null;
            }
            return staff;
        }
    }
}
