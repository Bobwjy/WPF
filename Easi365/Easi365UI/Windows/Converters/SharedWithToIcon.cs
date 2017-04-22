using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Easi365UI.Windows.Converters
{
    public class SharedWithToIcon : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int sharedWith = (int)value;

            if (sharedWith < 1)
                return new BitmapImage(new Uri("/Assets/UI/private.png", UriKind.Relative));
            else
                return new BitmapImage(new Uri("/Assets/UI/shared.png", UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
