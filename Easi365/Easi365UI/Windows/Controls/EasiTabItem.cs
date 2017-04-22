using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Easi365UI.Windows.Controls
{
    public class EasiTabItem : TabItem
    {
        public static readonly DependencyProperty MyMoverBrushProperty;
        public static readonly DependencyProperty MyEnterBrushProperty;

        public Brush MyMoverBrush
        {
            get
            {
                return base.GetValue(EasiTabItem.MyMoverBrushProperty) as Brush;
            }
            set
            {
                base.SetValue(EasiTabItem.MyMoverBrushProperty, value);
            }
        }

        public Brush MyEnterBrush
        {
            get
            {
                return base.GetValue(EasiTabItem.MyEnterBrushProperty) as Brush;
            }
            set
            {
                base.SetValue(EasiTabItem.MyEnterBrushProperty, value);
            }
        }

        public static DependencyProperty HeaderTextProperty = DependencyProperty.Register("MyHeaderText", typeof(string),
            typeof(EasiTabItem), new PropertyMetadata(""));

        public static DependencyProperty HeaderIconProperty = DependencyProperty.Register("HeaderIcon", typeof(Image),
            typeof(EasiTabItem),new PropertyMetadata(null));

        public string MyHeaderText
        {
            get { return GetValue(HeaderTextProperty) as string; }
            set { SetValue(HeaderTextProperty, value); }
        }

        public Image HeaderIcon
        {
            get { return GetValue(HeaderIconProperty) as Image; }
            set { SetValue(HeaderIconProperty, value); }
        }

        static EasiTabItem()
        {
            EasiTabItem.MyMoverBrushProperty = DependencyProperty.Register("MyMoverBrush", typeof(Brush), typeof(EasiTabItem), new PropertyMetadata(null));
            EasiTabItem.MyEnterBrushProperty = DependencyProperty.Register("MyEnterBrush", typeof(Brush), typeof(EasiTabItem), new PropertyMetadata(null));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(EasiTabItem), new FrameworkPropertyMetadata(typeof(EasiTabItem)));
        }

        public EasiTabItem()
        {
            //base.Header = "测试按钮";
            //base.Background = Brushes.Blue;
        }
    }
}
