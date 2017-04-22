using ClientLib.Entities;
using ClientLib.Services;
using Easi365UI.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Input;

namespace Easi365UI
{
    /// <summary>
    /// EditStaffWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StaffWindow : EasiWindow
    {
        object _obj;
        Action<object> _dialogResult;

        public StaffWindow()
        {
            InitializeComponent();

            //点击窗体头部区域拖拽整个窗体移动
            this.ContentHeader.MouseLeftButtonDown += HeaderGrid_MouseLeftButtonDown;
        }

        public StaffWindow(Action<object> dialogResult,string deptName)
            : this()
        {
            _dialogResult = dialogResult;
            this.txtDept.Text = deptName;
        }

        public StaffWindow(object obj, Action<object> dialogResult)
            : this()
        {
            _obj = obj;
            _dialogResult = dialogResult;
        }

        private void StaffWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_obj != null)
            {
                if (_obj is Staff)
                {
                    var staff = _obj as Staff;
                    this.wTitle.Text = "编辑人员: " + staff.UserName;
                    this.txtAccount.Text = staff.Account;
                    this.txtStaffName.Text = staff.UserName;
                    this.txtTelPhone.Text = staff.TelPhone;
                    this.txtDept.Text = staff.Dept;
                }
            }
        }

        void HeaderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //最小化登录窗体
        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        //关闭登录窗体
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtAccount.Text) || string.IsNullOrEmpty(txtStaffName.Text)) {
                return;
            }

            this.btnSave.IsEnabled = false;
            this.btnCancel.IsEnabled = false;

            var staff = _obj as Staff;
            if (staff != null)
            {
                staff.Account = this.txtAccount.Text;
                staff.UserName = this.txtStaffName.Text;
                staff.TelPhone = this.txtTelPhone.Text;
                staff.Dept = this.txtDept.Text;
            }
            else {
                staff = new Staff(this.txtAccount.Text, this.txtStaffName.Text, this.txtTelPhone.Text, this.txtDept.Text);
            }
            _dialogResult(staff);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
