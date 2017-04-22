using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Easi365UI.Util
{
    public class PasswordBoxBindingHelper
    {
        public static bool GetIsPasswordBindingEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPasswordBindingEnabledProperty);
        }

        public static void SetIsPasswordBindingEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPasswordBindingEnabledProperty, value);
        }

        public static readonly DependencyProperty IsPasswordBindingEnabledProperty = DependencyProperty.RegisterAttached("IsPasswordBindingEnabled", typeof(bool),
        typeof(PasswordBoxBindingHelper), new UIPropertyMetadata(false, OnIsPasswordBindingEnabledChanged));
        private static void OnIsPasswordBindingEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = obj as PasswordBox;
            if (passwordBox != null)
            {
                passwordBox.PasswordChanged -= PasswordBoxPasswordChanged;
                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordBoxPasswordChanged;
                }
            }
        }
        //when the passwordBox's password changed, update the buffer  
        static void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            if (!String.Equals(GetBindedPassword(passwordBox), passwordBox.Password))
            {
                SetBindedPassword(passwordBox, passwordBox.Password);
            }
        }

        public static string GetBindedPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(BindedPasswordProperty);
        }

        public static void SetBindedPassword(DependencyObject obj, string value)
        {
            obj.SetValue(BindedPasswordProperty, value);
        }

        public static readonly DependencyProperty BindedPasswordProperty = DependencyProperty.RegisterAttached("BindedPassword", typeof(string), typeof(PasswordBoxBindingHelper), new UIPropertyMetadata(string.Empty, OnBindedPasswordChanged));
        //when the buffer changed, upate the passwordBox's password  
        private static void OnBindedPasswordChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = obj as PasswordBox;
            if (passwordBox != null)
            {
                if (e.NewValue != null)
                {
                    string password = e.NewValue.ToString();
                    int len = password.Length;

                    passwordBox.Password = password;
                    //使用附加属性绑密码控件后 输入密码时光标永远停留在开始的位置
                    //使用这个方法将光标移动到输入文本的最后一个字符之后
                    SetPasswordBoxSelection(passwordBox, len, 0);
                }
                else
                    passwordBox.Password = string.Empty;
            }   
        }

        private static void SetPasswordBoxSelection(PasswordBox passwordBox, int start, int length)
        {
            var selectFuc = passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic);
            selectFuc.Invoke(passwordBox, new object[] { start, length });
        }
    }
}
