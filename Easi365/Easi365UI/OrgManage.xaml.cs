using Easi365UI.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace Easi365UI
{
    /// <summary>
    /// OrgManage.xaml 的交互逻辑
    /// </summary>
    public partial class OrgManage : EasiWindow
    {
        public OrgManage()
        {
            InitializeComponent();

            //点击窗体头部区域拖拽整个窗体移动
            this.ContentHeader.MouseLeftButtonDown += HeaderGrid_MouseLeftButtonDown;
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

        void HeaderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
