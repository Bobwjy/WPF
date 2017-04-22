using Easi365UI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Easi365UI.Models.Commands
{
    public class OpenMainWindowCommand : ICommand
    {

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (parameter != null && parameter.GetType() == typeof(MaxWindow))
            {
                MaxWindow window = (MaxWindow)parameter;
               
                if (!window.IsVisible)
                    window.Show();

                if (window.WindowState == System.Windows.WindowState.Minimized)
                    window.WindowState = System.Windows.WindowState.Normal;

                window.Activate();
                window.Topmost = true; 
                window.Topmost = false; 
                window.Focus();         
            }
        }
    }
}
