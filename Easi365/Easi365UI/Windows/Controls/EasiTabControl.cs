using System;
using System.Windows;
using System.Windows.Controls;

namespace Easi365UI.Windows.Controls
{
    public class EasiTabControl:TabControl
    {
        static EasiTabControl()
		{
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(EasiTabControl), new FrameworkPropertyMetadata(typeof(EasiTabControl)));
		}
    }
}
