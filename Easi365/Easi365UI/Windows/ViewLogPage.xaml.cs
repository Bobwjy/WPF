using ClientLib;
using ClientLib.SyncLogService;
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
using ClientLib.Core;

namespace Easi365UI.Windows
{
    /// <summary>
    /// ViewLogPage.xaml 的交互逻辑
    /// </summary>
    public partial class ViewLogPage : EasiWindow
    {
        public ObservableCollection<Log> Logs { get; set; }

        public ViewLogPage()
        {
            InitializeComponent();

            Logs = new ObservableCollection<Log>();
            this.DataContext = this;

            Dictionary<string, string> mydic = new Dictionary<string, string>() { 
                {"",""},
            {UserAction.Upload.ToString(),"上传"},
            {UserAction.Download.ToString(),"下载"},
            {UserAction.Delete.ToString(),"删除"},
            {UserAction.Send.ToString(),"发送"},
            {UserAction.Share.ToString(),"共享"},
            {UserAction.Update.ToString(),"更新"}
            };
            CbUserActions.ItemsSource = mydic;
            CbUserActions.SelectedValuePath = "Key";
            CbUserActions.DisplayMemberPath = "Value";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var logs = await GetAllLogsAsync();
                foreach (Log log in logs)
                    Logs.Add(log);
            }
            catch (Exception ex)
            {
                Logging.Add("获取日志信息失败.", ex);
                MessageBox.Show("获取日志信息失败.");
            }
        }

        private Task<IEnumerable<Log>> GetAllLogsAsync()
        {
            return Task.Factory.StartNew<IEnumerable<Log>>(() =>
            {
                LoggingService ser = new LoggingService();
                //http://192.168.0.118:10240/Service/LoggingService.asmx
                Uri uri = new Uri(CoreManager.ConfigManager.Settings.LocalServerUrl);
                string serverUrl = uri.Scheme + "://" + uri.Host;

                ser.Url = serverUrl + ":10240/Service/LoggingService.asmx";
                ser.Timeout = 5000;
                var logs = ser.GetAllLogs();
                return logs.ToList();
            });
        }

        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ucSearching.Visibility = System.Windows.Visibility.Visible;
                var startDate = dpStartDate.SelectedDate;
                var endDate = dpEndDate.SelectedDate;

                if (startDate != null && endDate != null)
                {
                    if (startDate.Value > endDate.Value)
                    {
                        MessageBox.Show("开始时间不能大于结束时间.");
                        return;
                    }
                }

                Logs.Clear();
                var logs = await GetAllLogsAsync();

                if (startDate.HasValue)
                    logs = logs.Where(s => s.CreatedTime > startDate.Value);

                if (endDate.HasValue)
                    logs = logs.Where(s => s.CreatedTime < endDate.Value);

                var selectedKey = Convert.ToString(CbUserActions.SelectedValue);
                if (!string.IsNullOrEmpty(selectedKey))
                {
                    var userAction = (UserAction)Enum.Parse(typeof(UserAction), selectedKey);

                    logs = logs
                            .AsQueryable()
                            .Where(s => s.UserAction == userAction);
                }

                logs.ToList().ForEach(c => Logs.Add(c));
                
            }
            catch (Exception ex)
            {
                Logging.Add("获取日志信息失败.", ex);
                MessageBox.Show("获取日志信息失败.");
            }
            finally
            {
                ucSearching.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
