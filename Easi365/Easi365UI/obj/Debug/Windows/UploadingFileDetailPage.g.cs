﻿#pragma checksum "..\..\..\Windows\UploadingFileDetailPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B3C48F8D1804FDCAC12BC8B72879DAA3"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Easi365UI.Windows;
using Easi365UI.Windows.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Easi365UI.Windows {
    
    
    /// <summary>
    /// UploadingFileDetailPage
    /// </summary>
    public partial class UploadingFileDetailPage : Easi365UI.Windows.Controls.EasiWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 34 "..\..\..\Windows\UploadingFileDetailPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock wTitle;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\Windows\UploadingFileDetailPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Easi365UI.Windows.Controls.EasiButton CloseBtn;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\Windows\UploadingFileDetailPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl UploadFileTabControl;
        
        #line default
        #line hidden
        
        
        #line 150 "..\..\..\Windows\UploadingFileDetailPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Easi365UI.Windows.Loading ucDeleting;
        
        #line default
        #line hidden
        
        
        #line 154 "..\..\..\Windows\UploadingFileDetailPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel CommandPanel;
        
        #line default
        #line hidden
        
        
        #line 155 "..\..\..\Windows\UploadingFileDetailPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnClearErrors;
        
        #line default
        #line hidden
        
        
        #line 156 "..\..\..\Windows\UploadingFileDetailPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnClearUploadHistories;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Easi365UI;component/windows/uploadingfiledetailpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Windows\UploadingFileDetailPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.wTitle = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.CloseBtn = ((Easi365UI.Windows.Controls.EasiButton)(target));
            return;
            case 3:
            this.UploadFileTabControl = ((System.Windows.Controls.TabControl)(target));
            
            #line 54 "..\..\..\Windows\UploadingFileDetailPage.xaml"
            this.UploadFileTabControl.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.TabControl_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ucDeleting = ((Easi365UI.Windows.Loading)(target));
            return;
            case 5:
            this.CommandPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 6:
            this.btnClearErrors = ((System.Windows.Controls.Button)(target));
            
            #line 155 "..\..\..\Windows\UploadingFileDetailPage.xaml"
            this.btnClearErrors.Click += new System.Windows.RoutedEventHandler(this.btnClearErrors_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.btnClearUploadHistories = ((System.Windows.Controls.Button)(target));
            
            #line 156 "..\..\..\Windows\UploadingFileDetailPage.xaml"
            this.btnClearUploadHistories.Click += new System.Windows.RoutedEventHandler(this.btnClearUploadHistories_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

