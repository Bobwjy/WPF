using ClientLib.Core;
using ClientLib.Entities;
using ClientLib.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Easi365UI.UserControls
{
    /// <summary>
    /// OrgManageUC.xaml 的交互逻辑
    /// </summary>
    public partial class OrgManageUC : UserControl
    {
        TreeViewItem _selectedNode { get; set; }
        Department _selectedDept { get; set; }
        OrgService _orgService = null;
        public ObservableCollection<Department> DepartmentList { get; set; }

        public virtual void Init()
        {
            try
            {
            }
            catch { }
        }

        public OrgManageUC()
        {
            InitializeComponent();
            BindDeptTvm();
        }

        /// <summary>
        /// 绑定部门树结构
        /// </summary>
        public void BindDeptTvm()
        {
            DepartmentList = null;
            this.OrgLoadingUC.LoadingText = "正在加载...";
            this.OrgLoadingUC.Visibility = System.Windows.Visibility.Visible;
            Task bindTreeTask = new Task(() =>
            {
                if (_orgService == null)
                {
                    _orgService = OrgService.GetInstence(App.spCredentials, App.databasePath);
                    _orgService.Init();
                }

                var results = _orgService.BindDepts();
                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.DataContext = null;

                    DepartmentList = results;
                    this.DataContext = this;
                    this.OrgLoadingUC.Visibility = System.Windows.Visibility.Collapsed;
                }));
            });
            bindTreeTask.Start();
        }

        #region 右键选中当前TreeViewItem
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _selectedNode = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (_selectedNode != null)
            {
                _selectedNode.Focus();
                e.Handled = true;
            }
        }
        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }
        #endregion

        #region TreeView 右键菜单方法
        /// <summary>
        /// 添加部门
        /// </summary>
        //private void Dept_Add(object sender, RoutedEventArgs e)
        //{
        //    DeptWindow dw = null;
        //    dw = new DeptWindow(async s =>
        //    {
        //        var depeartment = s as Department;
        //        depeartment.ParentID = _selectedDept.ID;
        //        depeartment.ParentName = _selectedDept.DeptName;

        //        dw.loading.Visibility = System.Windows.Visibility.Visible;
        //        var sucess = await _orgService.AddSubDept(depeartment);
        //        dw.loading.Visibility = System.Windows.Visibility.Collapsed;

        //        if (sucess)
        //        {
        //            _selectedDept.InsertSubDept(DepartmentList, depeartment);
        //            dw.DialogResult = true;
        //        }
        //        else
        //        {
        //            MessageBox.Show("创建部门失败.");
        //            dw.btnAddSubDept.IsEnabled = true;
        //            dw.CancelAddSubDept.IsEnabled = true;
        //        }
        //    }, _selectedDept);
        //    dw.ShowDialog();
        //}

        /// <summary>
        /// 修改部门
        /// </summary>
        //private void Dept_Edit(object sender, RoutedEventArgs e)
        //{
        //    DeptWindow dw = null;
        //    dw = new DeptWindow(_selectedNode.DataContext, async s =>
        //    {
        //        var depeartment = s as Department;
        //        dw.loading.Visibility = System.Windows.Visibility.Visible;
        //        var sucess = await _orgService.UpdateSubDept(depeartment);
        //        dw.loading.Visibility = System.Windows.Visibility.Collapsed;

        //        if (sucess)
        //        {
        //            depeartment.OriginalManager = depeartment.SpaceManager;
        //            dw.DialogResult = true;
        //        }
        //        else
        //        {
        //            MessageBox.Show("编辑部门失败.");
        //            dw.btnAddSubDept.IsEnabled = true;
        //            dw.CancelAddSubDept.IsEnabled = true;
        //        }
        //    }, "编辑部门");
        //    dw.ShowDialog();
        //}

        /// <summary>
        /// 判断是否可以执行删除
        /// </summary>
        private void DelDeptCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_selectedDept.Depts != null && _selectedDept.Depts.Count == 0)
            {
                if (!_orgService.HasStaffOfDept(_selectedDept.ID))
                {
                    e.CanExecute = true;
                }
            }
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        private async void Dept_Del(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("确定删除该部门吗？", "系统提示",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
            {
                this.loading.Visibility = System.Windows.Visibility.Visible;
                bool flag = await _orgService.DeleteSubDept(_selectedDept);
                _selectedDept.ParentDept.Depts.Remove(_selectedDept);
                this.loading.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void RefreshDepts(object sender, RoutedEventArgs e)
        {
            BindDeptTvm();
        }
        #endregion

        #region ListView右键菜单
        /// <summary>
        /// 添加人员
        /// </summary>
        //private void Staff_Add(object sender, RoutedEventArgs e)
        //{
        //    StaffWindow sw = null;
        //    try
        //    {
        //        sw = new StaffWindow(async s =>
        //        {
        //            var staff = s as Staff;
        //            staff.CurrentDeptID = Convert.ToInt32(_selectedDept.ID);

        //            sw.loading.Visibility = System.Windows.Visibility.Visible;
        //            var result = await _orgService.AddStaff(staff);
        //            sw.loading.Visibility = System.Windows.Visibility.Collapsed;

        //            if (result > 0)
        //            {
        //                LoadUsers();
        //                sw.DialogResult = true;
        //            }
        //            else
        //            {
        //                MessageBox.Show("添加人员失败.");
        //                sw.loading.Visibility = System.Windows.Visibility.Collapsed;
        //            }
        //        }, _selectedDept.DeptName);
        //        sw.ShowDialog();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.Add("添加人员失败", ex);
        //        sw.loading.Visibility = System.Windows.Visibility.Collapsed;
        //    }
        //}

        /// <summary>
        /// 编辑人员信息
        /// </summary>
        //private void EidtStaff(object sender, RoutedEventArgs e)
        //{
        //    if (StaffLvw.SelectedIndex > -1)
        //    {
        //        if (StaffLvw.SelectedItems.Count > 1)
        //        {
        //            MessageBox.Show("每次只能编辑一个项目.");
        //        }
        //        else
        //        {
        //            StaffWindow sw = null;
        //            sw = new StaffWindow((Staff)StaffLvw.SelectedItem, async s =>
        //            {
        //                sw.loading.Visibility = System.Windows.Visibility.Visible;
        //                var staff = s as Staff;
        //                staff.CurrentDeptID = Convert.ToInt32(_selectedDept.ID);

        //                var success = await _orgService.UpdateStaffById(staff);
        //                sw.loading.Visibility = System.Windows.Visibility.Collapsed;

        //                if (success)
        //                {
        //                    LoadUsers();
        //                    MessageBox.Show("编辑成功");
        //                    sw.DialogResult = true;
        //                }
        //                else {
        //                    MessageBox.Show("编辑失败");
        //                    sw.loading.Visibility = System.Windows.Visibility.Collapsed;
        //                }
        //            });
        //            sw.ShowDialog();
        //        }
        //    }
        //}

        /// <summary>
        /// 删除人员
        /// </summary>
        private void DeleteStaff(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var sItem in StaffLvw.SelectedItems)
                {
                    Staff staff = new Staff();
                    staff = (Staff)sItem;

                    if (System.Windows.Forms.MessageBox.Show("确定删除" + staff.UserName + "吗？", "系统提示",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                    {
                        var deptId = Convert.ToInt32(_selectedDept.ID);
                        if (!_orgService.DelStaffById(staff, deptId))
                        {
                            MessageBox.Show("删除人员失败: 姓名:" + staff.UserName + "(ID:" + staff.ID + ").");
                        }
                    }
                }
                LoadUsers();
            }
            catch { }
        }
        #endregion

        private void DeptsTvw_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _selectedDept = (Department)this.DeptsTvw.SelectedItem;

            LoadUsers();
        }

        private async void LoadUsers()
        {
            if (_selectedDept != null)
            {
                this.OrgLoadingUC.Visibility = System.Windows.Visibility.Visible;
                var results = await _orgService.GetStaffsByDeptIdFromLocal(_selectedDept.ID);
                if (results != null) {
                    this.StaffLvw.DataContext = results;
                } else {
                    this.StaffLvw.DataContext = results;
                }
                this.OrgLoadingUC.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
