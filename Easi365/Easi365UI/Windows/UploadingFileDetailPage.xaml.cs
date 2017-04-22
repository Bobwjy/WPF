using ClientLib;
using ClientLib.Entities;
using Easi365UI.Models;
using Easi365UI.Windows.Controls;
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

namespace Easi365UI.Windows
{
    /// <summary>
    /// UploadingFileDetailPage.xaml 的交互逻辑
    /// </summary>
    public partial class UploadingFileDetailPage : EasiWindow
    {
        LocalDB _db;

        public ObservableCollection<UploadFileModel> UploadingItems { get; private set; }
        public ObservableCollection<UploadedFile> UploadedItems { get; private set; }

        public UploadingFileDetailPage()
        {
            InitializeComponent();

            UploadingItems = new ObservableCollection<UploadFileModel>();
            UploadedItems = new ObservableCollection<UploadedFile>();
            this.DataContext = this;

            SetCommandPanelButton(UploadFileTabControl.SelectedIndex);
        }

        public void SetLocalDB(LocalDB db) 
        {
            if (_db == null)
                _db = db;

            var uploadedFiles = _db.GetUploadedFiles();
            uploadedFiles.ToList()
                .ForEach(c => UploadedItems.Add(c));
        }

        public void AddUploadedItems(ServerItem si)
        {
            if (si == null)
                throw new ArgumentNullException("ServerItem For UploadingFileDetailPage.xaml");

            UploadedFile uploadedFile = UploadedFile.GetUploadedFileByServerItem(si);
            UploadedItems.Add(uploadedFile);
        }

        public void RemoveCompletedUploadingFile()
        {
            var copiedCollection = new ObservableCollection<UploadFileModel>(UploadingItems);

            var completedFile = copiedCollection
                .Where(c => c.Status == "完成")
                .ToList();

            if (completedFile.Count > 0)
                completedFile
                    .ForEach(i => UploadingItems.Remove(i));

            var count = UploadingItems.Count;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public void ShowSelfInOneWindowCenter(Window window)
        {
            if (window != null)
            {
                this.Owner = window;
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

                this.Show();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl self = (TabControl)sender;
            SetCommandPanelButton(self.SelectedIndex);
        }

        private void SetCommandPanelButton(int index)
        {
            btnClearErrors.Visibility = System.Windows.Visibility.Collapsed;
            btnClearUploadHistories.Visibility = System.Windows.Visibility.Collapsed;

            if (index == 0)
                btnClearErrors.Visibility = System.Windows.Visibility.Visible;

            if(index == 1)
                btnClearUploadHistories.Visibility = System.Windows.Visibility.Visible;
        }

        private void btnClearUploadHistories_Click(object sender, RoutedEventArgs e)
        {
            DialogWindow dialog = new DialogWindow(async() =>
            {
                this.ucDeleting.Visibility = System.Windows.Visibility.Visible;
                await _db.ClearUploadedHistoryAsync();
                UploadedItems.Clear();
                this.ucDeleting.Visibility = System.Windows.Visibility.Collapsed;
            });
            dialog.TipText = "确定要删除所有的历史记录吗？";
            dialog.ShowSelfInOneWindowCenter(this);
        }

        private void btnClearErrors_Click(object sender, RoutedEventArgs e)
        {
            var copiedCollection = new ObservableCollection<UploadFileModel>(UploadingItems);

            var completedFile = copiedCollection
                .Where(c => c.Status == "失败")
                .ToList();

            if (completedFile.Count > 0)
                completedFile
                    .ForEach(i => UploadingItems.Remove(i));
        }
    }
}
