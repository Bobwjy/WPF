using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Easi365UI.Windows.Controls
{
    public class EasiWindow : Window
    {
        public EasiWindow()
        {
          //  this.DefaultStyleKey = typeof(EasiWindow);
            //缩放，最大化修复
            WindowBehaviorHelper wh = new WindowBehaviorHelper(this);
            wh.RepairBehavior();

          // this.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(EasiWindow_MouseLeftButtonDown);


            #if NET4
                this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.CloseWindowCommand, OnCloseWindow));
                this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
                this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
                this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
            #else
                this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.CloseWindowCommand, OnCloseWindow));
                this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
                this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
                this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
            #endif
        }

        //private void EasiWindow_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    var originSource = e.OriginalSource as Grid;
        //    if (originSource != null)
        //    {
        //        if (originSource.Name == "HeaderGrid")
        //            this.DragMove();

        //        e.Handled = true;
        //    }
        //}

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            //#if NET4
            //Microsoft.Windows.Shell.SystemCommands.CloseWindow(this);
            //#else
            //SystemCommands.CloseWindow(this);
            //#endif

            Microsoft.Windows.Shell.SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            //#if NET4
            //Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(this);
            //#else
            //SystemCommands.MaximizeWindow(this);
            //#endif

            Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            //#if NET4
            //Microsoft.Windows.Shell.SystemCommands.MinimizeWindow(this);
            //#else
            //SystemCommands.MinimizeWindow(this);
            //#endif

            Microsoft.Windows.Shell.SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            //#if NET4
            //Microsoft.Windows.Shell.SystemCommands.RestoreWindow(this);
            //#else
            //SystemCommands.RestoreWindow(this);
            //#endif

            Microsoft.Windows.Shell.SystemCommands.RestoreWindow(this);
        }
    }
}
