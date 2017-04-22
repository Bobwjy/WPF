using ClientLib.Core;
using ClientLib.Entities;
using Easi365UI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
    /// DeptWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeptWindow : EasiWindow
    {
        object _obj;
        Action<object> _dialogResult;
        Department pDept;
        string _title;

        public DeptWindow()
        {
            InitializeComponent();

            //点击窗体头部区域拖拽整个窗体移动
            this.ContentHeader.MouseLeftButtonDown += HeaderGrid_MouseLeftButtonDown;
        }

        void HeaderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public DeptWindow(Action<object> dialogResult, Department parentDept)
            : this()
        {
            _dialogResult = dialogResult;
            pDept = parentDept;
            txtbParentDept.Text = parentDept.DeptName;
        }

        public DeptWindow(object obj, Action<object> dialogResult, string title)
            : this()
        {
            _obj = obj;
            _dialogResult = dialogResult;
            _title = title;
        }

        private void DeptWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_obj != null)
            {
                if (_obj is Department)
                {
                    var dept = _obj as Department;
                    this.wTitle.Text = _title + ": " + dept.DeptName;
                    if (dept.ParentDept != null)
                        this.txtbParentDept.Text = dept.ParentDept.DeptName;

                    this.txtSubDept.Text = dept.DeptName;
                    if (dept.SpaceManager != null)
                        this.txtAdmin.Users = new ObservableCollection<CheckedUser>(dept.SpaceManager);
                }
            }
        }

        private void btnAddSubDept_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSubDept.Text) ||
                txtAdmin.Users == null || txtAdmin.Users.Count == 0)
            {
                return;
            }

            var users = ClientLib.Utilities.Common.GetUserValues(
                        CoreManager.ConfigManager.Settings.ServerUrl,
                        App.spCredentials,
                        txtAdmin.Users);
            if (users == null)
            {
                MessageBox.Show("用户不存在");
                return;
            }

            this.btnAddSubDept.IsEnabled = false;
            this.CancelAddSubDept.IsEnabled = false;

            var dept = _obj as Department;
            if (dept != null)
            {
                dept.DeptName = this.txtSubDept.Text;
                dept.SpaceManager = this.txtAdmin.Users;
            }
            else
            {
                dept = new Department(this.txtSubDept.Text, this.txtAdmin.Users, pDept);
            }
            _dialogResult(dept);
        }

        //最小化登录窗体
        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        //关闭登录窗体
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
