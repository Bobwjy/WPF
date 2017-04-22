using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Easi365UI.Windows.Controls
{
    public class EasiButton : Button
    {
        public static readonly DependencyProperty MyMoverBrushProperty;
        public static readonly DependencyProperty MyEnterBrushProperty;
        public Brush MyMoverBrush
        {
            get
            {
                return base.GetValue(EasiButton.MyMoverBrushProperty) as Brush;
            }
            set
            {
                base.SetValue(EasiButton.MyMoverBrushProperty, value);
            }
        }
        public Brush MyEnterBrush
        {
            get
            {
                return base.GetValue(EasiButton.MyEnterBrushProperty) as Brush;
            }
            set
            {
                base.SetValue(EasiButton.MyEnterBrushProperty, value);
            }
        }
        static EasiButton()
        {
            EasiButton.MyMoverBrushProperty = DependencyProperty.Register("MyMoverBrush", typeof(Brush), typeof(EasiButton), new PropertyMetadata(null));
            EasiButton.MyEnterBrushProperty = DependencyProperty.Register("MyEnterBrush", typeof(Brush), typeof(EasiButton), new PropertyMetadata(null));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(EasiButton), new FrameworkPropertyMetadata(typeof(EasiButton)));
        }
        public EasiButton()
        {
            base.Content = "";
            //base.Background = Brushes.Orange;
        }
    }
}
