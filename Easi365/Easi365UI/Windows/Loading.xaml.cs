using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Easi365UI.Windows
{
    /// <summary>
    /// Loading.xaml 的交互逻辑
    /// </summary>
    public partial class Loading : UserControl
    {
        public Loading()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LodingTextProperty = DependencyProperty.Register(
            "LoadingText",
            typeof(string), typeof(Loading), new PropertyMetadata(new PropertyChangedCallback(ChangeText)));

        public string LoadingText
        {
            get { return (string)this.GetValue(LodingTextProperty); }
            set { this.SetValue(LodingTextProperty, value); }
        }

        private static void ChangeText(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Loading self = (Loading)sender;
            self.lodingText.Text = Convert.ToString(e.NewValue);
        }
    }
}
