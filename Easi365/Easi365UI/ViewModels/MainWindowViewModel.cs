using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Windows;

namespace Easi365UI.ViewModels
{
    public class MainWindowViewModel : Screen
    {
        public MainWindowViewModel()
        { }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);

            var mainWindow = view as Window;
            mainWindow.Loaded += mainWindow_Loaded;
            mainWindow.SizeChanged += mainWindow_SizeChanged;
        }

        void mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var win = (Window)sender;

        }

        void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Window win = (Window)sender;
            double left = App.screenX - (win.Width + 60);
            double top = App.screenY - win.Height;

            if (top <= 0 || top <= 100) top = 0;
            else if (top <= 200) top = top / 3;
            else { top = 80; }

            win.Left = left;
            win.Top = top;
        }
    }
}
