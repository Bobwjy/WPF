using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ClientLib.Utilities;

namespace Easi365UI.Windows.Converters
{
    public class FileSizeFormat : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int fileSize;
            if (int.TryParse(System.Convert.ToString(values[0]), out fileSize))
            {
                bool isDirectory = (bool)values[1];

                return isDirectory ? "" : String.Format(new FileSizeFormatProvider(), "{0:fs1}", fileSize);
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
