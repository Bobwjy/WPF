using ClientLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IO = System.IO;

namespace Easi365UI.Windows.Converters
{
    public class StringToImage : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string fileName = (string)values[0];
            bool isDirectory = (bool)values[1];
            
            if (isDirectory) return new BitmapImage(new Uri("/Assets/UI/Large/folder.png", UriKind.Relative));

            string imageUri = "/Assets/UI/Large/unknow.png";
            string extension = IO.Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                return new BitmapImage(new Uri("/Assets/UI/Large/unkown.png", UriKind.Relative));

            extension = extension.TrimStart('.');

            if (Constants.FileExtensions.Contains(extension)) imageUri = string.Format("/Assets/UI/Large/{0}.png", extension);
            else if (Constants.NoteExtensions.Contains(extension)) imageUri = "/Assets/UI/Large/note.png";
            else imageUri = "/Assets/UI/Large/unkown.png";

            return new BitmapImage(new Uri(imageUri, UriKind.Relative));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
