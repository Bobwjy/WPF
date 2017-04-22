using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Easi365UI.Windows.Converters
{
    [ValueConversion(typeof(Int32), typeof(Visibility))]
    public class NumberToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //FolderIsEmpty 文件夹是否为空
            //UploadingFilesCount 正在上传的文件数量
            string param = System.Convert.ToString(parameter);
            
            int count = (int)value;
            if (string.Compare(param, "FolderIsEmpty", true) == 0)
                return count > 0 ? Visibility.Collapsed : Visibility.Visible;

            if (string.Compare(param, "UploadingFilesCount", true) == 0)
                return count > 0 ? Visibility.Visible : Visibility.Collapsed;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
