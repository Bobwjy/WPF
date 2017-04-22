using Easi365UI.Windows.Controls;
using System;
using System.Collections.Generic;
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

namespace Easi365UI.Windows
{
    /// <summary>
    /// FileNamePicker.xaml 的交互逻辑
    /// </summary>
    public partial class FileNamePicker : EasiWindow
    {
        Action<string> _setNameFunc;
        string _oldName;
        string[] _folderNames;

        public FileNamePicker()
        {
            InitializeComponent();
        }

        public FileNamePicker(Action<string> fuc,string[] folderNames)
            : this()
        {
            _setNameFunc = fuc;
            _folderNames = folderNames;
        }

        public FileNamePicker(Action<string> fuc, string oldName, string[] folderNames)
            : this()
        {
            _setNameFunc = fuc;
            _oldName = oldName;
            _folderNames = folderNames;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.FileName.Focus();
            if (!string.IsNullOrEmpty(_oldName))
            {
                this.FileName.Text = _oldName;
                this.FileName.SelectAll();
            }
        }

        //取消按钮事件
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        //确定按钮事件
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string foldername = this.FileName.Text;
            if (string.IsNullOrEmpty(foldername))
            {
                MessageBox.Show("名称不能为空");
                return;
            }
            if (_folderNames.Contains(foldername))
            {
                MessageBox.Show("名称已被使用");
                return;
            }

            _setNameFunc(foldername);

            this.DialogResult = true;
            this.Close();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
