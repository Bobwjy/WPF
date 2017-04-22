using ClientLib.Entities;
using SP = Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using System.Net;
using System.Collections.ObjectModel;
using ClientLib.Core;
using System.Collections;
using ClientLib.Utilities;
using System.Threading.Tasks;
using System.Data.OleDb;
using ClientLib.Common;

namespace ClientLib.Services
{
    public class OrgService
    {
        protected ClientContext _clientCtx { get; set; }
        protected ICredentials _credential;
        protected string _serverRoot;
        protected LocalDB _db;

        protected string _deptListTitle;
        protected string _staffListTitle;
        protected string _orgRelListTitle;
        private static string staffListTitle = "StaffList";

        protected SP.List _staffList = null;
        Dictionary<int, List<int>> _deptStaffDict = new Dictionary<int, List<int>>();

        private string _depeartRoot;
        private CreateWebService _createWebService;
        private static DbHelperOleDb dbHelper { get; set; }

        private static OrgService OrgManager = null;
        private OrgService(ICredentials credentials)
        {
            _serverRoot = CoreManager.ConfigManager.Settings.ServerUrl;
            _credential = credentials;

            this._deptListTitle = "DeptList";
            this._staffListTitle = "StaffList";
            this._orgRelListTitle = "OrgRelList";

            _depeartRoot = string.Format("{0}/{1}", _serverRoot, Constants.SpaceSite.Depeartment);
        }

        public static OrgService GetInstence(ICredentials credentials, string databasePath = "")
        {
            if (OrgManager == null)
            {
                OrgManager = new OrgService(credentials);
                if (databasePath != "") dbHelper = new DbHelperOleDb(databasePath);
            }
            return OrgManager;
        }

        public virtual void Init()
        {
            try
            {
                //1. 初始化客户端对象模型
                this._clientCtx = new ClientContext(_serverRoot);
                this._clientCtx.Credentials = _credential;

                //_staffList = _clientCtx.Web.Lists.GetByTitle(_staffListTitle);
            }
            catch { }
        }

        #region 组织架构显示
        public IEnumerable GetSubDepartments(int pid, bool nodeIsDept = true)
        {
            var list = GetDeptList(pid, nodeIsDept).Where(o => o.ParentID == pid).ToList();
            return list;
        }

        public List<OrgNode> GetDeptList(int pid, bool nodeIsDept)
        {
            List<OrgNode> orgList = new List<OrgNode>();
            //获取部门下的人员
            if (nodeIsDept)
            {
                orgList = GetStaffsList(orgList, pid);

                //    SP.List list = _clientCtx.Web.Lists.GetByTitle("DeptList");
                //    SP.CamlQuery camlQuery = new SP.CamlQuery();
                //    camlQuery.ViewXml = string.Format(@"<View>
                //                                        <Query>
                //                                            <Where>
                //                                            <Eq>
                //                                                <FieldRef Name='ParentID'/>
                //                                                <Value Type='Integer'>{0}</Value>
                //                                            </Eq>
                //                                            </Where>
                //                                        </Query>
                //                                    </View>", pid);
                //    SP.ListItemCollection items = list.GetItems(camlQuery);
                //    _clientCtx.Load(items);
                //    _clientCtx.ExecuteQuery();

                //    foreach (SP.ListItem listItem in items)
                //    {
                //        OrgNode org = new OrgNode
                //        {
                //            ID = listItem.Id,
                //            NodeName = "" + listItem["Title"],
                //            NodeIsDept = true,
                //            ParentDept = "" + listItem["ParentDept"],
                //            ParentID = (listItem["ParentID"] == null) ? 0 : Convert.ToInt32(listItem["ParentID"].ToString())
                //        };
                //        orgList.Add(org);
                //    }


                OleDbDataReader reader = null;
                var sqlStr = string.Format("SELECT * FROM DEPTS WHERE [ParentID] = {0}", pid);
                reader = dbHelper.ExecuteReader(sqlStr);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        OrgNode org = new OrgNode
                        {
                            ID = Convert.ToInt32(reader["ID"].ToString()),
                            NodeName = reader["Dept"].ToString(),
                            NodeIsDept = true,
                            ParentDept = reader["ParentDept"].ToString(),
                            ParentID = (reader["ParentID"] == null) ? 0 : Convert.ToInt32(reader["ParentID"].ToString())
                        };
                        orgList.Add(org);
                    }
                }

            }

            return orgList;
        }

        public List<OrgNode> GetStaffsList(List<OrgNode> orgList, int did)
        {
            //  SP.List list = _clientCtx.Web.Lists.GetByTitle("OrgRelList");
            //  SP.CamlQuery camlQuery = new SP.CamlQuery();
            //  camlQuery.ViewXml = string.Format(@"<View>
            //                                          <Query>
            //                                              <Where>
            //                                              <Eq>
            //                                                  <FieldRef Name='DeptID'/>
            //                                                  <Value Type='Integer'>{0}</Value>
            //                                              </Eq>
            //                                              </Where>
            //                                          </Query>
            //                                      </View>", did);
            //  SP.ListItemCollection items = list.GetItems(camlQuery);
            //  _clientCtx.Load(items);
            //  _clientCtx.ExecuteQuery();
            //  foreach (SP.ListItem listItem in items)
            //  {
            //      OrgNode org = new OrgNode
            //      {
            //          ID = listItem.Id,
            //          NodeName = "" + listItem["Title"],
            //          NodeIsDept = false,
            //          Account = GetAccount(Convert.ToInt32(listItem["StaffID"].ToString())),
            //          ParentID = did
            //      };
            //      orgList.Add(org);
            //  }

            OleDbDataReader reader = null;
            var sqlStr = string.Format("SELECT * FROM ORGREL WHERE [DEPTID] = {0}", did);
            reader = dbHelper.ExecuteReader(sqlStr);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Staff staff = GetStaffByid(Convert.ToInt32(reader["StaffID"].ToString()));
                    OrgNode org = new OrgNode
                    {
                        ID = staff.ID,
                        NodeName = staff.UserName,
                        NodeIsDept = false,
                        Account = staff.Account,
                        ParentID = did
                    };
                    orgList.Add(org);
                }
            }
            return orgList;
        }

        public Staff GetStaffByid(int userId)
        {
            Staff staff = null;
            OleDbDataReader reader = null;
            var sqlStr = string.Format("SELECT * FROM STAFFS WHERE [ID] = {0}", userId);
            reader = dbHelper.ExecuteReader(sqlStr);
            if (reader.HasRows)
            {
                if (reader.Read()) {
                    staff = new Staff
                    {
                        ID = userId,
                        UserName = reader["StaffName"].ToString(),
                        Account = reader["Account"].ToString()
                    };
                }
            }
            return staff;
        }
        #endregion

        #region 组织架构管理
        /// <summary>
        /// 获取部门列表
        /// </summary>
        public ObservableCollection<Department> GetDeptsFromServer()
        {
            try
            {
                SP.List list = _clientCtx.Web.Lists.GetByTitle(_deptListTitle);
                SP.CamlQuery query = SP.CamlQuery.CreateAllItemsQuery();
                SP.ListItemCollection items = list.GetItems(query);
                _clientCtx.Load(items, itms => itms.Include(
                    itm => itm.Id,
                    itm => itm.DisplayName,
                    itm => itm["ParentDept"],
                    itm => itm["ParentID"],
                    itm => itm["SpaceUri"],
                    itm => itm["SpaceManager"],
                    itm => itm["Modified"]
                ));
                _clientCtx.ExecuteQuery();

                ObservableCollection<Department> depts = new ObservableCollection<Department>();
                string id, deptName, parentName, parentId, spaceUri, modified;
                IList<CheckedUser> users = null;
                foreach (SP.ListItem listItem in items)
                {
                    id = listItem.Id.ToString();
                    deptName = Convert.ToString(listItem.DisplayName);
                    parentName = Convert.ToString(listItem["ParentDept"]);
                    parentId = Convert.ToString(listItem["ParentID"]);
                    spaceUri = Convert.ToString(listItem["SpaceUri"]);
                    modified = Convert.ToString(listItem["Modified"]);

                    if (listItem["SpaceManager"] != null)
                    {
                        users = new List<CheckedUser>();
                        var userVaues = listItem["SpaceManager"] as FieldUserValue[];
                        foreach (var userValue in userVaues)
                        {
                            var user = GetStaffByUserId(userValue.LookupId);
                            if (user != null)
                                users.Add(new CheckedUser { Id = user.ID, Account = user.Account, UserName = user.UserName });
                        }
                    }

                    Department n = new Department
                    {
                        ID = id,
                        DeptName = deptName,
                        ParentName = parentName,
                        ParentID = parentId,
                        SpaceUri = spaceUri,
                        Modified =  modified,
                        SpaceManager = users,
                        OriginalManager = users
                    };
                    depts.Add(n);
                }

                //ObservableCollection<Department> outputList = Bind(depts);
                //return outputList;

                return depts;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public List<CheckedUser> GetDeptManagerByDeptId(int deptId) {
            List<CheckedUser> users = new List<CheckedUser>();
            try
            {
                SP.List list = _clientCtx.Web.Lists.GetByTitle(_deptListTitle);
                SP.CamlQuery query = SP.CamlQuery.CreateAllItemsQuery();
                SP.ListItem item = list.GetItemById(deptId);
                _clientCtx.Load(item);
                _clientCtx.ExecuteQuery();

                if (item["SpaceManager"] != null)
                {
                    var userVaues = item["SpaceManager"] as FieldUserValue[];
                    foreach (var userValue in userVaues)
                    {
                        var user = GetStaffByUserId(userValue.LookupId);
                        if (user != null)
                            users.Add(new CheckedUser { Id = user.ID, Account = user.Account, UserName = user.UserName });
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                Logging.Add("获取部门管理员报错", ex);
                return null;
            }
        }

        public List<Staff> GetStaffsFromServer()
        {
            List<Staff> staffList = new List<Staff>();
            SP.List list = _clientCtx.Web.Lists.GetByTitle(_staffListTitle);
            SP.CamlQuery camlQuery = new SP.CamlQuery();
            camlQuery.ViewXml = string.Format(@"<View>
                                                    <Query>
                                                        <OrderBy><FieldRef Name='ID' Ascending='False' /></OrderBy>
                                                    </Query>
                                                </View>");
            SP.ListItemCollection items = list.GetItems(camlQuery);
            _clientCtx.Load(items);
            _clientCtx.ExecuteQuery();
            foreach (SP.ListItem listItem in items)
            {
                Staff staff = new Staff
                {
                    ID = listItem.Id,
                    UserName = Convert.ToString(listItem["Title"]),
                    Account = Convert.ToString(listItem["Account"]),
                    TelPhone = Convert.ToString(listItem["TelPhone"]),
                    Dept = Convert.ToString(listItem["Dept"]),
                    Modified = Convert.ToString(listItem["Modified"])
                };
                staffList.Add(staff);
            }
            return staffList;
        }

        /// <summary>
        /// 绑定树
        /// </summary>
        public ObservableCollection<Department> Bind(ObservableCollection<Department> depts)
        {
            ObservableCollection<Department> outputList = new ObservableCollection<Department>();
            for (int i = 0; i < depts.Count; i++)
            {
                if (depts[i].ParentID == "0")
                {
                    outputList.Add(depts[i]);
                }
                else
                {
                    var parent = FindChildNode(depts, depts[i].ParentID);
                    depts[i].ParentDept = parent;
                    parent.Depts.Add(depts[i]);
                }
            }
            return outputList;
        }

        /// <summary>
        /// 递归向下查找
        /// </summary>
        private Department FindChildNode(ObservableCollection<Department> depts, string pId)
        {
            if (depts == null) return null;
            for (int i = 0; i < depts.Count; i++)
            {
                if (depts[i].ID == pId)
                {
                    return depts[i];
                }
                Department _dept = FindChildNode(depts[i].Depts, pId);
                if (_dept != null)
                {
                    return _dept;
                }
            }
            return null;
        }

        /// <summary>
        /// 添加子部门
        /// </summary>
        /// <param name="dept">子部门</param>
        /// <returns>state</returns>
        public Task<bool> AddSubDept(Department dept)
        {
            return Task<bool>.Factory.StartNew(() =>
            {
                SP.ClientContext clientContext = null;
                SP.Web web = null;
                SP.ListItem newItem = null;

                int parentId = 0;
                int.TryParse(dept.ParentID, out parentId);
                try
                {
                    //部门列表数据
                    var users = ClientLib.Utilities.Common.GetUserValues(_clientCtx, dept.SpaceManager);
                    List deptList = _clientCtx.Web.Lists.GetByTitle(_deptListTitle);
                    ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                    newItem = deptList.AddItem(itemCreateInfo);
                    newItem["Title"] = dept.DeptName;
                    newItem["ParentID"] = parentId;
                    newItem["ParentDept"] = dept.ParentName;
                    newItem["SpaceUri"] = dept.SpaceUri;
                    newItem["SpaceManager"] = users;
                    newItem.Update();
                    _clientCtx.ExecuteQuery();

                    dept.ID = Convert.ToString(newItem.Id);

                    //添加空间
                    clientContext = new SP.ClientContext(_depeartRoot);
                    clientContext.Credentials = _credential;
                    _createWebService = new CreateWebService(clientContext);
                    web = _createWebService.Create(dept.SpaceUri, dept.DeptName, dept.DeptName);

                    //添加管理员
                    var principals = new List<SP.Principal>();
                    foreach (var userValue in users)
                    {
                        //var user = ClientLib.Utilities.Common.GetSiteUserById(clientContext, userValue.LookupId);
                        //if (user != null)
                        //    principals.Add(clientContext.Web.EnsureUser(user.LoginName.GetLoginNameWithoutDomain()));

                        principals.Add(ClientLib.Utilities.Common.GetUserByFieldUserValue(clientContext, userValue));
                    }
                    _createWebService.AddPremissions(web, principals, RoleType.Administrator);

                    ListItem item = deptList.GetItemById(Convert.ToInt32(dept.ID));
                    _clientCtx.Load(item);
                    _clientCtx.ExecuteQuery();
                    dept.Modified = item["Modified"].ToString();

                    InsertDeptInfoToLocal(dept);
                    return true;
                }
                catch (Exception ex)
                {
                    Logging.Add("添加部门报错", ex);

                    if (!string.IsNullOrEmpty(dept.ID))
                    {
                        try
                        {
                            newItem.DeleteObject();
                            _clientCtx.ExecuteQuery();
                        }
                        catch (Exception e)
                        {
                            Logging.Add("删除部门列表数据报错", e);
                        }
                    }

                    if (web != null)
                    {
                        try
                        {
                            web.DeleteObject();
                            web.Context.ExecuteQuery();
                        }
                        catch (Exception e)
                        {
                            Logging.Add("删除部门空间报错", e);
                        }
                    }
                }

                return false;
            });
        }

        /// <summary>
        /// 编辑子部门
        /// </summary>
        /// <param name="dept">子部门</param>
        /// <returns>state</returns>
        public Task<bool> UpdateSubDept(Department dept)
        {
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    string guid = string.Empty;

                    //部门列表数据
                    var users = ClientLib.Utilities.Common.GetUserValues(_clientCtx, dept.SpaceManager);
                    List deptList = _clientCtx.Web.Lists.GetByTitle(_deptListTitle);
                    ListItem updateItem = deptList.GetItemById(dept.ID);
                    updateItem["Title"] = dept.DeptName;
                    updateItem["SpaceManager"] = users;
                    updateItem.Update();
                    _clientCtx.ExecuteQuery();

                    if (!string.IsNullOrEmpty(dept.SpaceUri))
                    {
                        //修改网站标题
                        SP.ClientContext clientContext = new SP.ClientContext(string.Format("{0}/{1}", _depeartRoot, dept.SpaceUri));
                        clientContext.Credentials = _credential;
                        clientContext.Web.Title = dept.DeptName;
                        clientContext.Web.Update();
                        clientContext.ExecuteQuery();

                        //移除权限
                        _createWebService = new CreateWebService(clientContext);
                        var originalUsers = ClientLib.Utilities.Common.GetUserValues(_clientCtx, dept.OriginalManager);
                        if (originalUsers != null && originalUsers.Count() > 0)
                        {
                            var originalPrincipals = new List<SP.Principal>();
                            foreach (var userValue in originalUsers)
                            {
                                originalPrincipals.Add(ClientLib.Utilities.Common.GetUserByFieldUserValue(clientContext, userValue));
                            }
                            _createWebService.RemovePremissions(originalPrincipals);
                        }

                        //添加权限
                        var principals = new List<SP.Principal>();
                        foreach (var userValue in users)
                        {
                            principals.Add(ClientLib.Utilities.Common.GetUserByFieldUserValue(clientContext, userValue));
                        }
                        _createWebService.AddPremissions(clientContext.Web, principals, RoleType.Administrator);
                    }

                    ListItem item = deptList.GetItemById(Convert.ToInt32(dept.ID));
                    _clientCtx.Load(item);
                    _clientCtx.ExecuteQuery();
                    dept.Modified = item["Modified"].ToString();

                    UpdateDeptInfoToLocal(dept);
                    return true;
                }
                catch (Exception ex)
                {
                    Logging.Add("修改部门报错", ex);
                }

                return false;
            });
        }

        /// <summary>
        /// 删除子部门
        /// </summary>
        /// <param name="depeartment">子部门</param>
        /// <returns>state</returns>
        public Task<bool> DeleteSubDept(Department depeartment)
        {
            return Task<bool>.Factory.StartNew(() =>
            {
                int id = int.TryParse(depeartment.ID, out id) ? id : 0;
                try
                {
                    //部门列表数据
                    List deptList = _clientCtx.Web.Lists.GetByTitle(_deptListTitle);
                    ListItem item = deptList.GetItemById(id);
                    _clientCtx.Load(item);
                    _clientCtx.ExecuteQuery();

                    if (item != null && item.Id > 0)
                    {
                        //删除列表数据
                        item.DeleteObject();
                        _clientCtx.ExecuteQuery();

                        //删除空间
                        SP.ClientContext clientContext = new SP.ClientContext(string.Format("{0}/{1}", _depeartRoot, depeartment.SpaceUri));
                        clientContext.Credentials = _credential;
                        clientContext.Web.DeleteObject();
                        clientContext.ExecuteQuery();
                    }

                    DeleteDeptFromLocal(id);
                    return true;
                }
                catch (Exception ex)
                {
                    Logging.Add("删除部门报错", ex);
                    return false;
                }
            });
        }

        /// <summary>
        /// 根据部门Id获取人员集合
        /// </summary>
        public Task<List<Staff>> GetStaffsByDeptId(string deptId)
        {
            List<Staff> _staffList = new List<Staff>();
            return Task<List<Staff>>.Factory.StartNew(() =>
            {
                try
                {
                    int dId = int.TryParse(deptId, out dId) ? dId : 0;
                    _deptStaffDict = GetDeptStaffDict(dId);
                    var staffIds = _deptStaffDict[dId];
                    foreach (int id in staffIds)
                    {
                        SP.List list = _clientCtx.Web.Lists.GetByTitle("StaffList");
                        SP.ListItem item = list.GetItemById(id);
                        _clientCtx.Load(item);
                        _clientCtx.ExecuteQuery();
                        if (item != null)
                        {
                            Staff s = new Staff();
                            s.ID = item.Id;
                            s.UserName = Convert.ToString(item["Title"]);
                            s.Account = Convert.ToString(item["Account"]);
                            s.TelPhone = Convert.ToString(item["TelPhone"]);
                            s.Dept = Convert.ToString(item["Dept"]);
                            _staffList.Add(s);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Add("读取人员信息报错", ex);
                    return null;
                }
                return _staffList;
            });
        }

        /// <summary>
        /// 根据部门Id获取部门人员字典
        /// </summary>
        public Dictionary<int, List<int>> GetDeptStaffDict(int deptId)
        {
            try
            {
                _deptStaffDict.Clear();
                OleDbDataReader reader = null;
                var sqlStr = string.Format("SELECT * FROM ORGREL WHERE [DEPTID] = {0}", deptId);
                reader = dbHelper.ExecuteReader(sqlStr);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int dId = Convert.ToInt32(reader["DeptID"].ToString());
                        int staffId = Convert.ToInt32(reader["StaffID"].ToString());

                        if (!_deptStaffDict.ContainsKey(dId))
                        {
                            _deptStaffDict.Add(dId, new List<int>() { staffId });
                        }
                        else
                        {
                            _deptStaffDict[dId].Add(staffId);
                        }
                    }
                }


                //    SP.List list = _clientCtx.Web.Lists.GetByTitle(_orgRelListTitle);
                //    SP.CamlQuery camlQuery = new SP.CamlQuery();
                //    camlQuery.ViewXml = string.Format(@"<View>
                //                                        <Query>
                //                                            <Where>
                //                                            <Eq>
                //                                                <FieldRef Name='DeptID'/>
                //                                                <Value Type='Integer'>{0}</Value>
                //                                            </Eq>
                //                                            </Where>
                //                                        </Query>
                //                                    </View>", deptId);
                //    SP.ListItemCollection items = list.GetItems(camlQuery);
                //    _clientCtx.Load(items);
                //    _clientCtx.ExecuteQuery();

                //    foreach (SP.ListItem listItem in items)
                //    {
                //        int dId = Convert.ToInt32(listItem["DeptID"]);
                //        int staffId = Convert.ToInt32(listItem["StaffID"]);
                //        if (!_deptStaffDict.ContainsKey(dId))
                //        {
                //            _deptStaffDict.Add(dId, new List<int>() { staffId });
                //        }
                //        else
                //        {
                //            _deptStaffDict[dId].Add(staffId);
                //        }
                //    }


            }
            catch (Exception ex)
            {
                Logging.Add("读取部门和人员关系映射表报错", ex);
                return null;
            }
            return _deptStaffDict;
        }

        /// <summary>
        /// 判断部门下是否有员工
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public bool HasStaffOfDept(string deptId)
        {
            try
            {
                int dId = int.TryParse(deptId, out dId) ? dId : 0;
                SP.List list = _clientCtx.Web.Lists.GetByTitle(_orgRelListTitle);
                SP.CamlQuery camlQuery = new SP.CamlQuery();
                camlQuery.ViewXml = string.Format(@"<View>
                                                <Query>
                                                    <Where>
                                                    <Eq>
                                                        <FieldRef Name='DeptID'/>
                                                        <Value Type='Integer'>{0}</Value>
                                                    </Eq>
                                                    </Where>
                                                </Query>
                                            </View>", deptId);
                SP.ListItemCollection listItems = list.GetItems(camlQuery);
                _clientCtx.Load(listItems, items => items.Include(item => item.Id));
                _clientCtx.ExecuteQuery();

                return listItems.Count > 0;
            }
            catch (Exception ex)
            {
                Logging.Add("判断部门下是否有员工异常", ex);

                return true;
            }
        }

        /// <summary>
        /// 添加人员
        /// </summary>
        public Task<int> AddStaff(Staff staff)
        {
            return Task<int>.Factory.StartNew(() =>
            {
                try
                {
                    List staffList = _clientCtx.Web.Lists.GetByTitle(_staffListTitle);
                    ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                    ListItem newItem = staffList.AddItem(itemCreateInfo);
                    newItem["Title"] = staff.UserName;
                    newItem["Account"] = staff.Account;
                    newItem["TelPhone"] = staff.TelPhone;
                    newItem["Dept"] = staff.Dept;
                    newItem.Update();
                    _clientCtx.ExecuteQuery();

                    var staffId = newItem.Id;
                    ListItem item = staffList.GetItemById(staffId);
                    _clientCtx.Load(item);
                    _clientCtx.ExecuteQuery();

                    staff.ID = staffId;
                    staff.Modified = item["Modified"].ToString();

                    if (staffId > 0)
                    {
                        var orgRelId =  AddOrgRel(staff.UserName, staffId, staff.CurrentDeptID);
                        if (orgRelId > 0)
                        {
                            // 写入本地数据库
                            InsertStaffInfoToLocal(staff);
                        }
                        else {
                            ListItem deleteItem = staffList.GetItemById(staffId);
                            deleteItem.DeleteObject();
                            _clientCtx.ExecuteQuery();
                        }
                    }

                    return staffId;
                }
                catch (Exception ex)
                {
                    Logging.Add("添加人员报错", ex);
                    return 0;  //添加失败
                }
            });
        }

        /// <summary>
        /// 添加部门与人员的映射关系
        /// </summary>
        public int AddOrgRel(string staffName, int staffId, int deptId)
        {
            try
            {
                List orgRelList = _clientCtx.Web.Lists.GetByTitle(_orgRelListTitle);
                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                ListItem newItem = orgRelList.AddItem(itemCreateInfo);
                newItem["Title"] = staffName;
                newItem["StaffID"] = staffId;
                newItem["DeptID"] = deptId;
                newItem.Update();
                _clientCtx.ExecuteQuery();

                return newItem.Id;
            }
            catch (Exception ex)
            {
                Logging.Add("添加部门和人员的映射关系报错", ex);
                return 0;
            }
        }

        /// <summary>
        /// 获取人员
        /// </summary>
        public Staff GetStaffById(int staffId)
        {
            Staff staff = new Staff();
            try
            {
                SP.List staffList = _clientCtx.Web.Lists.GetByTitle(_staffListTitle);
                ListItem listItem = staffList.GetItemById(staffId);
                _clientCtx.Load(listItem);
                _clientCtx.ExecuteQuery();

                staff.ID = listItem.Id;
                staff.UserName = Convert.ToString(listItem["Title"]);
                staff.Account = Convert.ToString(listItem["Account"]);
                staff.TelPhone = Convert.ToString(listItem["TelPhone"]);
                staff.Dept = Convert.ToString(listItem["Dept"]);
            }
            catch (Exception ex)
            {
                Logging.Add("获取人员报错", ex);
                return null;
            }
            return staff;
        }

        /// <summary>
        /// 获取人员
        /// </summary>
        public Staff GetStaffByUserId(int uid)
        {
            Staff staff = null;
            try
            {
                var user = ClientLib.Utilities.Common.GetSiteUserById(_clientCtx, uid);
                if (user != null)
                {
                    SP.List staffList = _clientCtx.Web.Lists.GetByTitle(_staffListTitle);
                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = string.Format("<View><Query><Where><Eq><FieldRef Name='Account'/><Value Type='Text'>{0}</Value></Eq></Where></Query><RowLimit>1</RowLimit></View>", user.LoginName.GetLoginNameWithoutDomain());
                    ListItemCollection listItems = staffList.GetItems(camlQuery);
                    _clientCtx.Load(
                         listItems,
                         items => items.Include(
                             item => item.Id,
                             item => item["Title"],
                             item => item["Account"],
                             item => item["TelPhone"],
                             item => item["Dept"]));
                    _clientCtx.ExecuteQuery();
                    if (listItems != null && listItems.Count() > 0)
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

        /// <summary>
        /// 获取人员
        /// </summary>
        public static int GetStaffIdByLoginName(ClientContext context, string loginName)
        {
            try
            {
                SP.List staffList = context.Web.Lists.GetByTitle(staffListTitle);
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = string.Format("<View><Query><Where><Eq><FieldRef Name='Account'/><Value Type='Text'>{0}</Value></Eq></Where></Query><RowLimit>1</RowLimit></View>", loginName);
                ListItemCollection listItems = staffList.GetItems(camlQuery);
                context.Load(
                     listItems,
                     items => items.Include(
                         item => item.Id));
                context.ExecuteQuery();
                if (listItems != null && listItems.Count() > 0)
                {
                    return listItems[0].Id;
                }
            }
            catch (Exception ex)
            {
                Logging.Add("获取人员报错", ex);
                return -1;
            }
            return -1;
        }

        /// <summary>
        /// 判断用户是否在某部门中
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <param name="currentUserId">当前登陆用户ID</param>
        /// <returns></returns>
        public bool IsInDepeartment(string deptId, string currentUserId)
        {
            try
            {
                SP.List staffList = _clientCtx.Web.Lists.GetByTitle(_orgRelListTitle);
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = string.Format(@"<View>
                                                    <Query>
                                                        <Where>
                                                            <And>
                                                                <Eq>
                                                                    <FieldRef Name='StaffID'/>
                                                                    <Value Type='Integer'>{0}</Value>
                                                                </Eq>
                                                                <Eq>
                                                                    <FieldRef Name='DeptID'/>
                                                                    <Value Type='Integer'>{1}</Value>
                                                                </Eq>
                                                             </And>
                                                        </Where>
                                                    </Query>
                                                </View>", currentUserId, deptId);
                ListItemCollection listItems = staffList.GetItems(camlQuery);
                _clientCtx.Load(listItems, items => items.Include(item => item.Id));
                _clientCtx.ExecuteQuery();

                return listItems != null && listItems.Count() > 0;
            }
            catch (Exception ex)
            {
                Logging.Add("获取人员报错", ex);
                return false;
            }
        }

        public Task<bool> UpdateStaffById(Staff staff)
        {
            return Task<bool>.Factory.StartNew(() =>
            {
                try
                {
                    SP.List staffList = _clientCtx.Web.Lists.GetByTitle(_staffListTitle);
                    ListItem listItem = staffList.GetItemById(Convert.ToInt32(staff.ID));
                    listItem["Title"] = staff.UserName;
                    listItem["Account"] = staff.Account;
                    listItem["TelPhone"] = staff.TelPhone;
                    listItem["Dept"] = staff.Dept;
                    listItem.Update();
                    _clientCtx.ExecuteQuery();

                    // 更新关系映射表
                    UpdateOrgRel(staff);

                    ListItem item = staffList.GetItemById(staff.ID);
                    _clientCtx.Load(item);
                    _clientCtx.ExecuteQuery();
                    staff.Modified = item["Modified"].ToString();

                    UpdateStaffInfoToLocal(staff);

                    return true;
                }
                catch (Exception ex)
                {
                    Logging.Add("更新人员报错", ex);
                    return false;
                }
            });
        }

        public void UpdateOrgRel(Staff staff)
        {
            try
            {
                SP.List orgRelList = _clientCtx.Web.Lists.GetByTitle(_orgRelListTitle);
                SP.CamlQuery camlQuery = new SP.CamlQuery();
                camlQuery.ViewXml = string.Format(@"<View>
                                                    <Query>
                                                        <Where>
                                                            <And>
                                                                <Eq>
                                                                    <FieldRef Name='StaffID'/>
                                                                    <Value Type='Integer'>{0}</Value>
                                                                </Eq>
                                                                <Eq>
                                                                    <FieldRef Name='DeptID'/>
                                                                    <Value Type='Integer'>{1}</Value>
                                                                </Eq>
                                                             </And>
                                                        </Where>
                                                    </Query>
                                                </View>", staff.ID, staff.CurrentDeptID);
                SP.ListItemCollection items = orgRelList.GetItems(camlQuery);
                _clientCtx.Load(items);
                _clientCtx.ExecuteQuery();

                if (items.Count > 0)
                {
                    ListItem listItem = items[0];
                    listItem["Title"] = staff.UserName;
                    listItem["StaffID"] = staff.ID;
                    //listItem["DeptID"] = staff.NewDeptID;
                    listItem.Update();
                    _clientCtx.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                Logging.Add("更新部门与人员映射关系报错", ex);
            }
        }

        /// <summary>
        /// 删除人员
        /// </summary>
        public bool DelStaffById(Staff staff, int deptId)
        {
            try
            {
                var staffId = Convert.ToInt32(staff.ID);
                List staffList = _clientCtx.Web.Lists.GetByTitle(_staffListTitle);
                ListItem listitem = staffList.GetItemById(staffId);
                listitem.DeleteObject();
                _clientCtx.ExecuteQuery();

                DelOrgRel(staffId, deptId);

                DeleteStaffFromLocal(staffId, deptId);
            }
            catch (Exception ex)
            {
                Logging.Add("删除人员报错: 姓名:" + staff.UserName + "(ID:" + staff.ID + ").", ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 删除部门和人员的映射关系
        /// </summary>
        public void DelOrgRel(int staffId, int deptId)
        {
            try
            {
                SP.List list = _clientCtx.Web.Lists.GetByTitle(_orgRelListTitle);
                SP.CamlQuery camlQuery = new SP.CamlQuery();
                camlQuery.ViewXml = string.Format(@"<View>
                                                    <Query>
                                                        <Where>
                                                            <And>
                                                                <Eq>
                                                                    <FieldRef Name='ID'/>
                                                                    <Value Type='Integer'>{0}</Value>
                                                                </Eq>
                                                                <Eq>
                                                                    <FieldRef Name='DeptID'/>
                                                                    <Value Type='Integer'>{1}</Value>
                                                                </Eq>
                                                             </And>
                                                        </Where>
                                                    </Query>
                                                </View>", staffId, deptId);
                SP.ListItemCollection items = list.GetItems(camlQuery);
                _clientCtx.Load(items);
                _clientCtx.ExecuteQuery();

                foreach (ListItem item in items)
                {
                    item.DeleteObject();
                    _clientCtx.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                Logging.Add("删除部门与人员的关系报错", ex);
            }
        }

        /// <summary>
        /// 获取组织架构关系映射表
        /// </summary>
        /// <returns></returns>
        public List<OrgRel> GetOrgRelList() {
            try
            {
                List<OrgRel> orgRelList = new List<OrgRel>();
                SP.List list = _clientCtx.Web.Lists.GetByTitle(_orgRelListTitle);
                SP.CamlQuery query = SP.CamlQuery.CreateAllItemsQuery();
                SP.ListItemCollection items = list.GetItems(query);
                _clientCtx.Load(items, itms => itms.Include(
                    itm => itm["Title"],
                    itm => itm["StaffID"],
                    itm => itm["DeptID"]
                ));
                _clientCtx.ExecuteQuery();

                foreach (SP.ListItem listItem in items)
                {
                    OrgRel or = new OrgRel();
                    or.StaffName = Convert.ToString(listItem["Title"]);
                    or.StaffId = Convert.ToInt32(listItem["StaffID"]);
                    or.DeptId = Convert.ToInt32(listItem["DeptID"]);
                    orgRelList.Add(or);
                }

                return orgRelList;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        
        #endregion

        #region 本地数据库操作
        public ObservableCollection<Department> BindDepts()
        {
            try
            {
                var depts = GetDeptsFromLocal();

                ObservableCollection<Department> outputList = Bind(depts);
                return outputList;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public ObservableCollection<Department> GetDeptsFromLocal()
        {
            try
            {
                ObservableCollection<Department> depts = new ObservableCollection<Department>();
                OleDbDataReader reader = null;
                var sqlStr = "SELECT * FROM DEPTS ORDER BY [ID] DESC";
                reader = dbHelper.ExecuteReader(sqlStr);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var users = GetDeptManagerByDeptId(Convert.ToInt32(reader["ID"].ToString()));
                        Department n = new Department
                        {
                            ID = reader["ID"].ToString(),
                            DeptName = reader["Dept"].ToString(),
                            ParentID = reader["ParentID"].ToString(),
                            SpaceUri = reader["SpaceUri"].ToString(),
                            Modified = reader["Modified"].ToString(),
                            SpaceManager = users,
                            OriginalManager = users 
                        };
                        depts.Add(n);
                    }
                }

                return depts;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        public void InsertDeptInfoToLocal(Department dept)
        {
            try
            {
                var insertSql = string.Format("INSERT INTO DEPTS([ID],[Dept],[ParentDept],[ParentID],[SpaceUri],[Modified]) " +
                                "VALUES({0},'{1}','{2}',{3},'{4}','{5}')", dept.ID, dept.DeptName, dept.ParentName, dept.ParentID, dept.SpaceUri, dept.Modified);
                dbHelper.ExecuteSql(insertSql);
            }
            catch (Exception ex)
            {
                Logging.Add("向本地数据库中添加部门信息时报错", ex);
            }
        }

        public void UpdateDeptInfoToLocal(Department dept)
        {
            try
            {
                var updateSql = string.Format("UPDATE DEPTS SET [Dept] = '{0}',[ParentDept] = '{1}',[ParentID] = {2},[SpaceUri] = '{3}',[Modified] = '{4}' " +
                                    "WHERE [ID] = {5}", dept.DeptName, dept.ParentName, dept.ParentID, dept.SpaceUri, dept.Modified, dept.ID);
                dbHelper.ExecuteSql(updateSql);
            }
            catch (Exception ex)
            {
                Logging.Add("向本地数据库中添加部门信息时报错", ex);
            }
        }

        public void DeleteDeptFromLocal(int deptId)
        {
            try
            {
                var deleteSql = string.Format("DELETE FROM DEPTS WHERE [ID] = {0}", deptId);
                dbHelper.ExecuteSql(deleteSql);
            }
            catch (Exception ex)
            {
                Logging.Add("删除本地数据库中部门时报错", ex);
            }
        }

        public Task<List<Staff>> GetStaffsByDeptIdFromLocal(string deptId)
        {
            List<Staff> _staffList = new List<Staff>();
            return Task<List<Staff>>.Factory.StartNew(() =>
            {
                try
                {
                    int dId = int.TryParse(deptId, out dId) ? dId : 0;
                    _deptStaffDict = GetDeptStaffDict(dId);
                    var staffIds = _deptStaffDict[dId];
                    foreach (int id in staffIds)
                    {
                        OleDbDataReader reader = null;
                        var sqlStr = string.Format("SELECT * FROM STAFFS WHERE [ID] = {0}", id);
                        reader = dbHelper.ExecuteReader(sqlStr);
                        if (reader.HasRows) {
                            if (reader.Read())
                            {
                                Staff s = new Staff();
                                s.ID = Convert.ToInt32(reader["ID"].ToString());
                                s.UserName = reader["StaffName"].ToString();
                                s.Account = reader["Account"].ToString();
                                s.TelPhone = reader["TelPhone"].ToString();
                                s.Dept = reader["Dept"].ToString();
                                _staffList.Add(s);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Add("读取人员信息报错", ex);
                    return null;
                }
                return _staffList;
            });
        }

        public Staff GetStaffByIdFromLocal(int staffId)
        {
            Staff staff = new Staff();
            try
            {
                OleDbDataReader reader = null;
                var sqlStr = string.Format("SELECT * FROM STAFFS WHERE [ID] = {0}", staffId);
                reader = dbHelper.ExecuteReader(sqlStr);
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        staff.UserName = Convert.ToString(reader["StaffName"]);
                        staff.Account = Convert.ToString(reader["Account"]);
                        staff.TelPhone = Convert.ToString(reader["TelPhone"]);
                        staff.Dept = Convert.ToString(reader["Dept"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Add("根据Id获取本地人员信息报错", ex);
                return null;
            }
            return staff;
        }

        public void InsertStaffInfoToLocal(Staff staff)
        {
            try
            {
                var insertSql = string.Format("INSERT INTO Staffs([ID],[StaffName],[Account],[TelPhone],[Dept],[Modified]) " +
                                "VALUES({0},'{1}','{2}','{3}','{4}','{5}')", staff.ID, staff.UserName, staff.Account, staff.TelPhone, staff.Dept, staff.Modified);
                var staffAdded = dbHelper.ExecuteSql(insertSql);

                if (staffAdded > 0)
                {
                    insertSql = string.Format("INSERT INTO ORGREL([StaffName],[StaffId],[DeptId]) VALUES('{0}', {1}, {2})", staff.UserName, staff.ID, staff.CurrentDeptID);
                    dbHelper.ExecuteSql(insertSql);
                }
            }
            catch (Exception ex)
            {
                Logging.Add("向本地数据库中添加人员信息时报错", ex);
            }
        }

        public void UpdateStaffInfoToLocal(Staff staff) {
            try
            {
                var updateSql = string.Format("UPDATE STAFFS SET [StaffName] = '{0}',[Account] = '{1}',[TelPhone] = '{2}',[Dept] = '{3}',[Modified] = '{4}' " +
                                    "WHERE [ID] = {5}", staff.UserName, staff.Account, staff.TelPhone, staff.Dept, staff.Modified, staff.ID);
                var staffUpdated = dbHelper.ExecuteSql(updateSql);

                if (staffUpdated > 0) {
                    updateSql = string.Format("UPDATE ORGREL SET [StaffName] = '{0}',[StaffId] = {1},[DeptId] = {2} " +
                                    "WHERE [StaffId] = {3} AND [DeptId] = {4}", staff.UserName, staff.ID, staff.CurrentDeptID, staff.ID, staff.CurrentDeptID);
                    dbHelper.ExecuteSql(updateSql);
                }
            }
            catch (Exception ex)
            {
                Logging.Add("向本地数据库中更新人员信息时报错", ex);
            }
        }

        public void DeleteStaffFromLocal(int staffId, int deptId)
        {
            try
            {
                var deleteSql = string.Format("DELETE FROM STAFFS WHERE [ID] = {0}", staffId);
                var staffDeleted = dbHelper.ExecuteSql(deleteSql);

                if (staffDeleted > 0)
                {
                    deleteSql = string.Format("DELETE FROM ORGREL WHERE [StaffId] = {0} AND [DeptId] = {1}", staffId,deptId);
                    dbHelper.ExecuteSql(deleteSql);
                }
            }
            catch (Exception ex)
            {
                Logging.Add("向本地数据库中更新人员信息时报错", ex);
            }
        }

        public List<Staff> GetStaffsFromLocal() {
            List<Staff> staffList = new List<Staff>();
            try
            {
                OleDbDataReader reader = null;
                var sqlStr = "SELECT * FROM STAFFS";
                reader = dbHelper.ExecuteReader(sqlStr);
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Staff s = new Staff();
                        s.ID = Convert.ToInt32(reader["ID"].ToString());
                        s.UserName = reader["StaffName"].ToString();
                        s.Account = reader["Account"].ToString();
                        s.TelPhone = reader["TelPhone"].ToString();
                        s.Dept = reader["Dept"].ToString();
                        s.Modified = reader["Modified"].ToString();
                        staffList.Add(s);
                    }
                }
                return staffList;
            }
            catch (Exception ex)
            {
                Logging.Add("获取本地所有人员报错", ex);
                return null;
            }
        }

        public Department GetParentDeptFromLocal(int deptId)
        {
            Department parentDept = null;
            try
            {
                var sqlStr = string.Format("SELECT [ParentID] FROM DEPTS WHERE [ID] = {0}", deptId);
                var parentId = Convert.ToInt32(dbHelper.GetSingle(sqlStr).ToString());

                OleDbDataReader reader = null;
                sqlStr = string.Format("SELECT * FROM DEPTS WHERE [ID] = {0}", parentId);
                reader = dbHelper.ExecuteReader(sqlStr);

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        parentDept = new Department
                        {
                            ID = reader["ID"].ToString(),
                            DeptName = reader["Dept"].ToString(),
                            ParentID = reader["ParentID"].ToString(),
                            SpaceUri = reader["SpaceUri"].ToString(),
                            Modified = reader["Modified"].ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Add("获取本地所有人员报错", ex);
            }
            return parentDept;
        }
        #endregion

    }


}
