using ClientLib.Core;
using ClientLib.Entities;
using ClientLib.Services;
using ClientLib.Utilities;
using Easi365UI.Windows.Controls;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Easi365UI
{
    /// <summary>
    /// SpaceManage.xaml 的交互逻辑
    /// </summary>
    public partial class SpaceManage : EasiWindow
    {
        private static readonly string CooSpace = "协作空间";
        private static readonly string DeptSpace = "部门空间";
        private string cooSpaceListRoot = "";
        private string deptSpaceListRoot = "";
        string webRoot = "", listTitle = "";

        public List<SpaceCategory> spaceCategoryList = null;
        CreateWebService createWebService;
        ObservableCollection<Spaces> spacesList = null;
        OrgService orgService = null;
        Spaces selectedSpace = null;

        ClientContext cooSpaceClientCtx = null;
        ClientContext deptSpaceClientCtx = null;

        MaxWindow mw = null;

        public SpaceManage(MaxWindow window)
        {
            InitializeComponent();
            mw = window;

            //点击窗体头部区域拖拽整个窗体移动
            this.ContentHeader.MouseLeftButtonDown += HeaderGrid_MouseLeftButtonDown;
        }

        private void SMWindow_Loaded(object sender, RoutedEventArgs e)
        {
            cooSpaceListRoot = string.Format("{0}/{1}", CoreManager.ConfigManager.Settings.ServerUrl, Constants.SpaceSite.CooSpace);
            deptSpaceListRoot = CoreManager.ConfigManager.Settings.ServerUrl;

            spaceCategoryList = new List<SpaceCategory> { 
                new SpaceCategory { Name = CooSpace,CategoryType = CoreManager.SpaceCategory.CooSpace}, 
                new SpaceCategory { Name = DeptSpace,CategoryType = CoreManager.SpaceCategory.DeptSpace}
            };

            this.SpaceCategoryLst.ItemsSource = spaceCategoryList;
            this.SpaceCategoryLst.SelectedItem = this.SpaceCategoryLst.Items[0];
        }

        void HeaderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //关闭登录窗体
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public class SpaceCategory
        {
            public string Name { get; set; }
            public CoreManager.SpaceCategory CategoryType { get; set; }
        }

        private void SpaceCategoryLst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BindSpacesList();
            SpaceCategoryLst.IsEnabled = false;
        }

        private async void BindSpacesList() {
            try
            {
                this.DataContext = null;
                this.Loading.Visibility = System.Windows.Visibility.Visible;
                SpaceCategory sc = SpaceCategoryLst.SelectedItem as SpaceCategory;
                switch (sc.CategoryType)
                {
                    case CoreManager.SpaceCategory.CooSpace:
                        webRoot = cooSpaceListRoot;
                        listTitle = Constants.SpaceSite.CooSpaceListTitle;
                        if (!SpaceGridView.Columns.Contains(ApplicantColumn)) SpaceGridView.Columns.Add(ApplicantColumn);
                        if (!SpaceGridView.Columns.Contains(AccountColumn)) SpaceGridView.Columns.Add(AccountColumn);
                        SpaceGridView.Columns.Remove(SpaceAdminsColumn);
                        SpaceGridView.Columns.Add(SpaceAdminsColumn);
                        break;
                    case CoreManager.SpaceCategory.DeptSpace:
                        webRoot = deptSpaceListRoot;
                        listTitle = Constants.SpaceSite.DeptSpaceListTitle;
                        if (SpaceGridView.Columns.Contains(ApplicantColumn)) SpaceGridView.Columns.Remove(ApplicantColumn);
                        if (SpaceGridView.Columns.Contains(AccountColumn)) SpaceGridView.Columns.Remove(AccountColumn);
                        break;
                    default:
                        webRoot = cooSpaceListRoot;
                        listTitle = Constants.SpaceSite.CooSpaceListTitle;
                        if (!SpaceGridView.Columns.Contains(ApplicantColumn)) SpaceGridView.Columns.Add(ApplicantColumn);
                        if (!SpaceGridView.Columns.Contains(AccountColumn)) SpaceGridView.Columns.Add(AccountColumn);
                        SpaceGridView.Columns.Remove(SpaceAdminsColumn);
                        SpaceGridView.Columns.Add(SpaceAdminsColumn);
                        break;
                }

                spacesList = new ObservableCollection<Spaces>();
                var tempList = await GetSpacesList(sc.CategoryType);
                foreach (var space in tempList) spacesList.Add(space);
                this.DataContext = spacesList;
                SpaceCategoryLst.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logging.Add("空间管理界面加载空间列表报错", ex);
            }
            finally {
                this.Loading.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public async Task<ObservableCollection<Spaces>> GetSpacesList(CoreManager.SpaceCategory spaceCategory)
        {
            ObservableCollection<Spaces> spacesList = null;
            try
            {
                if (spaceCategory == CoreManager.SpaceCategory.CooSpace)
                {
                    if (cooSpaceClientCtx == null)
                    {
                        cooSpaceClientCtx = new ClientContext(webRoot);
                        cooSpaceClientCtx.Credentials = App.spCredentials;
                    }
                    createWebService = new CreateWebService(cooSpaceClientCtx);
                    spacesList = await createWebService.GetSpacesList(listTitle);
                }

                if (spaceCategory == CoreManager.SpaceCategory.DeptSpace)
                {
                    if (deptSpaceClientCtx == null)
                    {
                        deptSpaceClientCtx = new ClientContext(webRoot);
                        deptSpaceClientCtx.Credentials = App.spCredentials;
                    }
                    createWebService = new CreateWebService(deptSpaceClientCtx);
                    spacesList = await createWebService.GetSpacesList(listTitle);
                }

                return spacesList;
            }
            catch { return null; }
        }

        private void SpaceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                selectedSpace = SpaceListView.SelectedItem as Spaces;
                if (selectedSpace.IsCreated == false)
                {
                    this.CreateSpace.Visibility = System.Windows.Visibility.Visible;
                    this.EditSpace.Visibility = System.Windows.Visibility.Collapsed;
                    this.DeleteSpace.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    this.CreateSpace.Visibility = System.Windows.Visibility.Collapsed;
                    this.EditSpace.Visibility = System.Windows.Visibility.Visible;
                    this.DeleteSpace.Visibility = System.Windows.Visibility.Visible;
                }

                if (selectedSpace.SpaceCategory == CoreManager.SpaceCategory.DeptSpace) this.DeleteSpace.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch { selectedSpace = null; }
        }

        private void CreateSpaceMenuEvent(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedSpace.ID))
            {
                if (selectedSpace.SpaceCategory == CoreManager.SpaceCategory.CooSpace)
                {
                    CreateCooSpace ccs = null;
                    ccs = new CreateCooSpace(c =>
                    {
                        BindSpacesList();
                        mw.BindSpacesList();
                    }, CoreManager.CooSpaceAction.New, selectedSpace.ID);
                    ccs.ShowDialog();
                }
            }
        }

        private void EditSpaceMenuEvent(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedSpace.ID))
            {
                if (selectedSpace.SpaceCategory == CoreManager.SpaceCategory.CooSpace)
                {
                    CreateCooSpace ccs = null;
                    ccs = new CreateCooSpace(c =>
                    {
                        BindSpacesList();
                    }, CoreManager.CooSpaceAction.Edit, selectedSpace.ID);
                    ccs.ShowDialog();
                }
                else
                {
                    orgService = OrgService.GetInstence(App.spCredentials);
                    orgService.Init();

                    var depeartment = new Department();
                    depeartment.ID = selectedSpace.ID;
                    depeartment.DeptName = selectedSpace.SpaceTitle;
                    depeartment.ParentDept = orgService.GetParentDeptFromLocal(Convert.ToInt32(selectedSpace.ID));
                    depeartment.SpaceManager = selectedSpace.SpaceAdmin;
                    depeartment.OriginalManager = selectedSpace.OriginalManager;

                    DeptWindow dw = null;
                    dw = new DeptWindow(depeartment, async s =>
                    {
                        dw.loading.Visibility = System.Windows.Visibility.Visible;
                        var sucess = await orgService.UpdateSubDept(depeartment);
                        dw.loading.Visibility = System.Windows.Visibility.Collapsed;

                        if (sucess)
                        {
                            depeartment.OriginalManager = depeartment.SpaceManager;
                            dw.DialogResult = true;
                        }
                        else
                        {
                            MessageBox.Show("编辑空间失败.");
                            dw.btnAddSubDept.IsEnabled = true;
                            dw.CancelAddSubDept.IsEnabled = true;
                        }
                    }, "编辑空间");
                    dw.ShowDialog();
                }
            }
        }

        private void DeleteSpaceMenuEvent(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedSpace.ID))
            {
                if (selectedSpace.SpaceCategory == CoreManager.SpaceCategory.CooSpace)
                {
                    DialogWindow dialog = new DialogWindow(() =>
                    {
                        createWebService.DeleteListDataForCooSpace(Convert.ToInt32(selectedSpace.ID));
                        string webUrl = string.Format("{0}/{1}/{2}", CoreManager.ConfigManager.Settings.ServerUrl,
                            Constants.SpaceSite.CooSpace, selectedSpace.SpaceUri);

                        createWebService.DeleteWeb(webUrl);

                        MoveItemFromCollection(selectedSpace.ID);

                        mw.BindSpacesList();
                    });
                    dialog.TipText = "确定要删除空间 " + selectedSpace.SpaceTitle + " 吗？";
                    dialog.ShowDialog();
                }
            }
        }

        private void MoveItemFromCollection(string id)
        {
            foreach (var item in spacesList)
            {
                if (item.ID == id)
                {
                    spacesList.Remove(item);
                    break;
                }
            }
        }

        private void UpdateItem(string id,Spaces space)
        {
            foreach (var item in spacesList)
            {
                if (item.ID == id)
                {
                    spacesList.Remove(item);
                    spacesList.Add(space);
                    break;
                }
            }
        }

    }
}
