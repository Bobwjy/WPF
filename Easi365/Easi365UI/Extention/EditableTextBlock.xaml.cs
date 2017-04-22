using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ClientLib.Entities;
using Easi365UI.Models;
using Easi365UI.Util;

namespace Easi365UI.Extention
{
    public partial class EditableTextBlock : UserControl
    {

        #region Constructor

        public EditableTextBlock()
        {
            InitializeComponent();
            base.Focusable = true;
            base.FocusVisualStyle = null;
        }

        #endregion Constructor

        #region Member Variables

        // We keep the old text when we go into editmode
        // in case the user aborts with the escape key
        private string oldText;

        #endregion Member Variables

        #region Properties
        /// <summary>
        /// 文件条目ID
        /// </summary>
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(int), typeof(EditableTextBlock), new PropertyMetadata(null));
        public int Id
        {
            get { return (int)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        /// <summary>
        /// 原始路径
        /// </summary>
        public static readonly DependencyProperty OriginalPathProperty = DependencyProperty.Register("OriginalPath", typeof(string), typeof(EditableTextBlock), new PropertyMetadata(null));
        public string OriginalPath
        {
            get { return (string)GetValue(OriginalPathProperty); }
            set { SetValue(OriginalPathProperty, value); }
        }

        /// <summary>
        /// 文本框内容
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableTextBlock), new PropertyMetadata(""));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 是否为目录
        /// </summary>
        public static readonly DependencyProperty IsDirectoryProperty = DependencyProperty.Register("IsDirectory", typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(true));
        public bool IsDirectory
        {
            get { return (bool)GetValue(IsDirectoryProperty); }
            set { SetValue(IsDirectoryProperty, value); }
        }
        
        /// <summary>
        /// 是否处于编辑状态
        /// </summary>
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(false));
        public bool IsInEditMode
        {
            get
            {
                return (bool)GetValue(IsInEditModeProperty);
            }
            set
            {
                if (value) oldText = Text;
                SetValue(IsInEditModeProperty, value);
            }
        }

        public static readonly DependencyProperty TextFormatProperty = DependencyProperty.Register("TextFormat", typeof(string), typeof(EditableTextBlock), new PropertyMetadata("{0}"));
        public string TextFormat
        {
            get { return (string)GetValue(TextFormatProperty); }
            set
            {
                if (value == "") value = "{0}";
                SetValue(TextFormatProperty, value);
            }
        }
        public string FormattedText
        {
            get { return String.Format(TextFormat, Text); }
        }

        /// <summary>
        /// 重命名操作
        /// </summary>
        public static readonly DependencyProperty Lost_FocusProperty = DependencyProperty.Register("Lost_Focus", typeof(Action<EditableTextBlock>), typeof(EditableTextBlock), new PropertyMetadata(null));
        public Action<EditableTextBlock> Lost_Focus
        {
            get { return (Action<EditableTextBlock>)GetValue(Lost_FocusProperty); }
            set { SetValue(Lost_FocusProperty, value); }
        }        

        #endregion Properties

        #region Event Handlers

        // Invoked when we enter edit mode.
        void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox txt = sender as TextBox;

            // Give the TextBox input focus
            txt.Focus();

            if (this.IsDirectory || txt.Text.LastIndexOf(".") < 0)
                txt.SelectAll();
            else
                txt.Select(0, txt.Text.LastIndexOf("."));
        }

        // Invoked when we exit edit mode.
        void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsInEditMode = false;

            if (Lost_Focus != null)
                Lost_Focus(this);
        }

        // Invoked when the user edits the annotation.
        void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.IsInEditMode = false;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                this.IsInEditMode = false;
                Text = oldText;
                e.Handled = true;
            }
        }

        #endregion Event Handlers

    }
}
