using ClientLib.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Easi365UI.Windows.Converters
{
    public class ToComboxRole : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            UserRole role;
            Enum.TryParse<UserRole>(System.Convert.ToString(value), out role);
            return (int)role;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string val = System.Convert.ToString(value);
            UserRole role;
            Enum.TryParse<UserRole>(val, out role);
            return role;
        }
    }
}
