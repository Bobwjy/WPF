using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Easi365UI.Windows.Converters
{
    [ValueConversion(typeof(Int32), typeof(String))]
    public class NumberToText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int num = (int)value;
            if (num == 0)
            {
                return "未找到相关联系人";
            }
            else if (num == -1)
            {
                return "正在搜索……";
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NumberToProgressValue : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int val;
            if(int.TryParse(System.Convert.ToString(value),out val))
            {
                if (val > 0)
                    return val;
            }

            return 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
