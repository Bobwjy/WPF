using ClientLib.Services;
using ClientLib.Utilities;
using Easi365UI.Windows.Controls;
using Microsoft.SharePoint.Client;
using System;
using System.Windows;
using System.Threading.Tasks;
using ClientLib.Core;
using ClientLib.Entities;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Input;

namespace Easi365UI
{
    /// <summary>
    /// CreateCooSpace.xaml 的交互逻辑
    /// </summary>
    public partial class CreateCooSpace : EasiWindow
    {
        protected string _serverRoot;
        private string _cooSpaceRoot;
        private string _spaceId;
        private string _spaceUri;
        CoreManager.CooSpaceAction _action;
        private CreateWebService _createWebService;
        Action<Spaces> _dialogResult;
        ClientContext clientContext = null;

        public CreateCooSpace()
        {
            InitializeComponent();

            //点击窗体头部区域拖拽整个窗体移动
            this.ContentHeader.MouseLeftButtonDown += HeaderGrid_MouseLeftButtonDown;
        }

        public CreateCooSpace(Action<Spaces> dialogResult,
            CoreManager.CooSpaceAction action = CoreManager.CooSpaceAction.Applicant, string spaceId = "0")
            : this()
        {
            _dialogResult = dialogResult;
            _action = action;
            _serverRoot = CoreManager.ConfigManager.Settings.ServerUrl;
            _cooSpaceRoot = string.Format("{0}/{1}", _serverRoot, Constants.SpaceSite.CooSpace);
            _spaceId = spaceId;

            clientContext = new ClientContext(_cooSpaceRoot);
            clientContext.Credentials = App.spCredentials;
            _createWebService = new CreateWebService(clientContext);

            switch (_action)
            {
                case CoreManager.CooSpaceAction.Applicant:
                    this.wTitle.Text = "申请协作空间";
                    this.btnCreateSpace.IsEnabled = true;
                    this.btnCancel.IsEnabled = true;
                    break;
                case CoreManager.CooSpaceAction.New:
                    this.wTitle.Text = "创建协作空间";
                    BindSpaceInfo();
                    break;
                case CoreManager.CooSpaceAction.Edit:
                    this.wTitle.Text = "编辑协作空间";
                    BindSpaceInfo();
                    break;
                case CoreManager.CooSpaceAction.View:
                    break;
            }
        }

        public virtual void Init() { }

        //关闭登录窗体
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void HeaderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private async void BindSpaceInfo() {
            this.ucLoading.Visibility = System.Windows.Visibility.Visible;
            this.ucLoading.LoadingText = "正在加载...";

            int spaceId = int.TryParse(_spaceId, out spaceId) ? spaceId : 0;
            var space = await _createWebService.GetCooSpaceInfoById(spaceId);

            this.txtCooSpaceName.Text = space.SpaceTitle;
            this.txtDescription.Text = space.Desc;
            this.txtSpaceAdmin.Users = new ObservableCollection<CheckedUser>(space.SpaceAdmin);
            this.txtSpaceMembers.Users = new ObservableCollection<CheckedUser>(space.SpaceMember);
            _spaceUri = space.SpaceUri;

            this.ucLoading.Visibility = System.Windows.Visibility.Collapsed;
            this.btnCreateSpace.IsEnabled = true;
            this.btnCancel.IsEnabled = true;
        }

        private async void btnCreateSpace_Click(object sender, RoutedEventArgs e)
        {
            string msg = "";
            try
            {
                var webUrl = Guid.NewGuid().ToString();
                var spaceTitle = this.txtCooSpaceName.Text;
                var spaceDesc = this.txtDescription.Text;
                var spaceAdmins = txtSpaceAdmin.Users;
                var spaceMembers = txtSpaceMembers.Users;
                Spaces cs = new Spaces
                {
                    SpaceTitle = spaceTitle,
                    Applicant = CoreManager.ConfigManager.Settings.CurrentUserName,
                    Account = CoreManager.ConfigManager.Settings.CurrentUserName,
                    IsCreated = false,
                    SpaceAdmin = spaceAdmins,
                    SpaceMember = spaceMembers,
                    SpaceUri = webUrl,
                    Desc = spaceDesc
                };
                if (string.IsNullOrEmpty(spaceTitle) || spaceAdmins == null || spaceAdmins.Count == 0)
                {
                    MessageBox.Show("空间名称、空间管理员不能为空!");
                    return;
                }

                var users = ClientLib.Utilities.Common.GetUserValues(
                            CoreManager.ConfigManager.Settings.ServerUrl,
                            App.spCredentials,
                            txtSpaceAdmin.Users);
                if (users == null)
                {
                    MessageBox.Show("用户不存在");
                    return;
                }

                var members = ClientLib.Utilities.Common.GetUserValues(
                            CoreManager.ConfigManager.Settings.ServerUrl,
                            App.spCredentials,
                            txtSpaceMembers.Users);
                if (members == null)
                {
                    MessageBox.Show("用户不存在");
                    return;
                }

                this.btnCreateSpace.IsEnabled = false;
                this.btnCancel.IsEnabled = false;

                this.ucLoading.Visibility = System.Windows.Visibility.Visible;
                this.ucLoading.LoadingText = "正在提交...";

                if (_action == CoreManager.CooSpaceAction.Applicant)
                {
                    if (await ApplicantSpace(cs))
                    {
                        _dialogResult(cs);
                        DialogResult = true;
                    }
                    else
                    {
                        msg = "申请协作空间失败.";
                        MessageBox.Show(msg);
                    }
                }
                else if (_action == CoreManager.CooSpaceAction.New)
                {
                    cs.ID = _spaceId;
                    cs.IsCreated = true;
                    if (await AddSpace(cs))
                    {
                        _dialogResult(cs);
                        DialogResult = true;
                    }
                    else
                    {
                        msg = "创建协作空间失败.";
                        MessageBox.Show(msg);
                    }
                }
                else
                {
                    cs.ID = _spaceId;
                    cs.IsCreated = true;
                    if (await UpdateSpace(cs))
                    {
                        _dialogResult(cs);
                        DialogResult = true;
                    }
                    else
                    {
                        msg = "编辑协作空间失败.";
                        MessageBox.Show(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Add(msg, ex);
            }
            finally {
                this.btnCreateSpace.IsEnabled = true;
                this.btnCancel.IsEnabled = true;
                this.ucLoading.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 创建协作空间
        /// </summary>
        /// <returns></returns>
        private Task<bool> ApplicantSpace(Spaces cs)
        {
            return Task<bool>.Factory.StartNew(() =>
            {
                ListItem newItem = null;
                try
                {
                    //添加数据到列表
                    var users = ClientLib.Utilities.Common.GetUserValues(clientContext, cs.SpaceAdmin);
                    var members = ClientLib.Utilities.Common.GetUserValues(clientContext, cs.SpaceMember);
                    List cooSpaceList = clientContext.Web.Lists.GetByTitle(Constants.SpaceSite.CooSpaceListTitle);
                    ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                    newItem = cooSpaceList.AddItem(itemCreateInfo);
                    newItem["Title"] = cs.SpaceTitle;
                    newItem["Applicant"] = cs.Applicant;
                    newItem["Account"] = cs.Account;
                    newItem["IsCreated"] = cs.IsCreated;
                    newItem["SpaceManager"] = users;
                    newItem["SpaceMember"] = members;
                    newItem["SpaceUri"] = cs.SpaceUri;
                    newItem["Description"] = cs.Desc;
                    newItem.Update();
                    clientContext.ExecuteQuery();
                    cs.ID = Convert.ToString(newItem.Id);

                    return true;
                }
                catch (Exception ex)
                {
                    Logging.Add("创建协作空间报错", ex);

                    if (!string.IsNullOrEmpty(cs.ID))
                    {
                        try
                        {
                            newItem.DeleteObject();
                            clientContext.ExecuteQuery();
                        }
                        catch (Exception e)
                        {
                            Logging.Add("删除部门列表数据报错", e);
                        }
                    }
                }
                return false;
            });
        }

        private Task<bool> AddSpace(Spaces cs)
        {
            return Task<bool>.Factory.StartNew(() =>
            {
                Web web = null;
                try
                {
                    var admin = ClientLib.Utilities.Common.GetUserValues(clientContext, cs.SpaceAdmin);
                    var members = ClientLib.Utilities.Common.GetUserValues(clientContext, cs.SpaceMember);

                    web = _createWebService.Create(_spaceUri, cs.SpaceTitle, cs.Desc);

                    //添加管理员权限
                    var adminPrincipals = new List<Principal>();
                    foreach (var userValue in admin)
                    {
                        adminPrincipals.Add(ClientLib.Utilities.Common.GetUserByFieldUserValue(clientContext, userValue));
                    }
                    _createWebService.AddPremissions(web, adminPrincipals, RoleType.Administrator);

                    //添加成员权限
                    var principals = new List<Principal>();
                    foreach (var userValue in members)
                    {
                        principals.Add(ClientLib.Utilities.Common.GetUserByFieldUserValue(clientContext, userValue));
                    }
                    _createWebService.AddPremissions(web, principals, RoleType.Contributor);

                    //更新列表数据
                    ListItem listItem = null;
                    List cooSpaceList = clientContext.Web.Lists.GetByTitle(Constants.SpaceSite.CooSpaceListTitle);
                    int spaceId = int.TryParse(cs.ID, out spaceId) ? spaceId : 0;
                    listItem = cooSpaceList.GetItemById(spaceId);
                    listItem["Title"] = cs.SpaceTitle;
                    listItem["IsCreated"] = cs.IsCreated;
                    listItem["SpaceManager"] = admin;
                    listItem["SpaceMember"] = members;
                    listItem["Description"] = cs.Desc;
                    listItem.Update();
                    clientContext.ExecuteQuery();

                    return true;
                }
                catch (Exception ex)
                {
                    Logging.Add("创建协作空间报错", ex);

                    if (web != null)
                    {
                        try
                        {
                            web.DeleteObject();
                            web.Context.ExecuteQuery();
                        }
                        catch (Exception e)
                        {
                            Logging.Add("删除协作空间报错", e);
                        }
                    }
                }

                return false;
            });
        }

        private Task<bool> UpdateSpace(Spaces cs) {
            return Task<bool>.Factory.StartNew(() =>
            {
                ListItem listItem = null;

                try
                {
                    var admin = ClientLib.Utilities.Common.GetUserValues(clientContext, cs.SpaceAdmin);
                    var members = ClientLib.Utilities.Common.GetUserValues(clientContext, cs.SpaceMember);

                    string webUrl = string.Format("{0}/{1}", _cooSpaceRoot, _spaceUri);
                    //ClientContext context = _createWebService.UpdateWeb(webUrl, cs.SpaceTitle, cs.Desc);
                    ClientContext context = new ClientContext(webUrl);
                    context.Credentials = App.spCredentials;
                    context.Web.Title = cs.SpaceTitle;
                    context.Web.Description = cs.Desc;
                    context.Web.Update();
                    context.ExecuteQuery();

                    _createWebService = new CreateWebService(context);

                    //移除管理员权限
                    var oriAdminPrincipals = new List<Principal>();
                    foreach (var userValue in admin)
                    {
                        oriAdminPrincipals.Add(ClientLib.Utilities.Common.GetUserByFieldUserValue(context, userValue));
                    }
                    _createWebService.RemovePremissions(oriAdminPrincipals);

                    //移除成员权限
                    var oriMembersPrincipals = new List<Principal>();
                    foreach (var userValue in members)
                    {
                        oriMembersPrincipals.Add(ClientLib.Utilities.Common.GetUserByFieldUserValue(context, userValue));
                    }
                    _createWebService.RemovePremissions(oriMembersPrincipals);


                    //添加管理员权限
                    var adminPrincipals = new List<Principal>();
                    foreach (var userValue in admin)
                    {
                        adminPrincipals.Add(ClientLib.Utilities.Common.GetUserByFieldUserValue(context, userValue));
                    }
                    _createWebService.AddPremissions(context.Web, adminPrincipals, RoleType.Administrator);

                    //添加成员权限
                    var principals = new List<Principal>();
                    foreach (var userValue in members)
                    {
                        principals.Add(ClientLib.Utilities.Common.GetUserByFieldUserValue(context, userValue));
                    }
                    _createWebService.AddPremissions(context.Web, principals, RoleType.Contributor);


                    //更新列表数据
                    clientContext = new ClientContext(_cooSpaceRoot);
                    clientContext.Credentials = App.spCredentials;
                    List cooSpaceList = clientContext.Web.Lists.GetByTitle(Constants.SpaceSite.CooSpaceListTitle);
                    int spaceId = int.TryParse(cs.ID, out spaceId) ? spaceId : 0;
                    listItem = cooSpaceList.GetItemById(spaceId);
                    listItem["Title"] = cs.SpaceTitle;
                    listItem["IsCreated"] = cs.IsCreated;
                    listItem["SpaceManager"] = admin;
                    listItem["SpaceMember"] = members;
                    listItem["Description"] = cs.Desc;
                    listItem.Update();
                    clientContext.ExecuteQuery();

                    return true;
                }
                catch (Exception ex)
                {
                    Logging.Add("编辑协作空间报错", ex);
                }
                return false;
            });
        }

    }
}
