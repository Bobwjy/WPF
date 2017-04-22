using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Easi365UI.Windows.Controls
{
    public class Easi365Window : Window
    {
        public Easi365Window()
        {
            this.DefaultStyleKey = typeof(Easi365Window);
            this.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(ModernWindow_MouseLeftButtonDown);
        }

        void ModernWindow_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //this.DragMove(); 
        }
    }
}
