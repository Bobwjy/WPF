using Easi365UI.Windows.Controls;
using System;
using System.Windows;
using System.Windows.Input;

namespace Easi365UI
{
    /// <summary>
    /// DialogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DialogWindow : EasiWindow
    {
        Action _callback;
        public enum DialogType
        { 
            Alert,
            Confirm
        }

        public static readonly DependencyProperty TipTextProperty = DependencyProperty.Register(
            "TipText",
            typeof(string), typeof(DialogWindow), new PropertyMetadata(new PropertyChangedCallback(ChangeText)));

        public string TipText
        {
            get { return (string)this.GetValue(TipTextProperty); }
            set { this.SetValue(TipTextProperty, value); }
        }

        private static void ChangeText(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DialogWindow self = (DialogWindow)sender;
            self.tbMessage.Text = Convert.ToString(e.NewValue);
        }

        public DialogWindow()
        {
            InitializeComponent();

            //点击窗体头部区域拖拽整个窗体移动
            this.ContentHeader.MouseLeftButtonDown += HeaderGrid_MouseLeftButtonDown;
        }

        void HeaderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public DialogWindow(Action callback, DialogType dt = DialogType.Confirm)
            : this()
        {
            this._callback = callback;

            switch (dt)
            {
                case DialogType.Confirm:
                    break;
                case DialogType.Alert:
                    btnYes.Content = "确 认";
                    btnYes.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    btnNo.Visibility = System.Windows.Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }

        //关闭登录窗体
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }
        
        /// <summary>
        /// 确定事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            if (_callback != null)
                _callback();

            this.DialogResult = true;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        /// <summary>
        /// 居中显示当前窗体
        /// </summary>
        /// <param name="window"></param>
        public void ShowSelfInOneWindowCenter(Window window)
        {
            if (window != null)
            {
                this.Owner = window;
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

                this.ShowDialog();
            }
        }
    }
}
