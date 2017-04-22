using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Easi365UI.Windows.Controls
{
    public class EasiLinkLable : Label
    {
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(Uri), typeof(EasiLinkLable));

        [Category("Common Properties"), Bindable(true)]
        public Uri Url
        {
            get { return GetValue(UrlProperty) as Uri; }
            set { SetValue(UrlProperty, value); }
        }

        public static readonly DependencyProperty HyperlinkStyleProperty =
            DependencyProperty.Register("HyperlinkStyle", typeof(Style),
                typeof(EasiLinkLable));

        public Style HyperlinkStyle
        {
            get { return GetValue(HyperlinkStyleProperty) as Style; }
            set { SetValue(HyperlinkStyleProperty, value); }
        }

        public static readonly DependencyProperty HoverForegroundProperty =
            DependencyProperty.Register("HoverForeground", typeof(Brush),
                typeof(EasiLinkLable));

        [Category("Brushes"), Bindable(true)]
        public Brush HoverForeground
        {
            get { return GetValue(HoverForegroundProperty) as Brush; }
            set { SetValue(HoverForegroundProperty, value); }
        }

        public static readonly DependencyProperty LinkLabelBehaviorProperty =
            DependencyProperty.Register("LinkLabelBehavior",
                typeof(LinkLabelBehavior),
                typeof(EasiLinkLable));

        [Category("Common Properties"), Bindable(true)]
        public LinkLabelBehavior LinkLabelBehavior
        {
            get { return (LinkLabelBehavior)GetValue(LinkLabelBehaviorProperty); }
            set { SetValue(LinkLabelBehaviorProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EasiLinkLable));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EasiLinkLable));

        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(EasiLinkLable));

        [Localizability(LocalizationCategory.NeverLocalize), Bindable(true), Category("Action")]
        public object CommandParameter
        {
            get { return this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        [Localizability(LocalizationCategory.NeverLocalize), Bindable(true), Category("Action")]
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        [Bindable(true), Category("Action")]
        public IInputElement CommandTarget
        {
            get { return (IInputElement)this.GetValue(CommandTargetProperty); }
            set { this.SetValue(CommandTargetProperty, value); }
        }

        [Category("Behavior")]
        public static readonly RoutedEvent ClickEvent;

        [Category("Behavior")]
        public static readonly RoutedEvent RequestNavigateEvent;

        static EasiLinkLable()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(EasiLinkLable),
                new FrameworkPropertyMetadata(typeof(EasiLinkLable)));

            ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EasiLinkLable));
            RequestNavigateEvent = EventManager.RegisterRoutedEvent("RequestNavigate", RoutingStrategy.Bubble, typeof(RequestNavigateEventHandler), typeof(EasiLinkLable));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Hyperlink innerHyperlink = GetTemplateChild("PART_InnerHyperlink") as Hyperlink;
            if (innerHyperlink != null)
            {
                innerHyperlink.Click += new RoutedEventHandler(InnerHyperlink_Click);
                innerHyperlink.RequestNavigate += new RequestNavigateEventHandler(InnerHyperlink_RequestNavigate);
            }
        }

        void InnerHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            RequestNavigateEventArgs args = new RequestNavigateEventArgs(e.Uri, String.Empty);
            args.Source = this;
            args.RoutedEvent = EasiLinkLable.RequestNavigateEvent;
            RaiseEvent(args);
        }


        void InnerHyperlink_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(EasiLinkLable.ClickEvent, this));
        }

        public event RoutedEventHandler Click
        {
            add
            {
                base.AddHandler(ClickEvent, value);
            }
            remove
            {
                base.RemoveHandler(ClickEvent, value);
            }
        }

        public event RequestNavigateEventHandler RequestNavigate
        {
            add
            {
                base.AddHandler(RequestNavigateEvent, value);
            }
            remove
            {
                base.RemoveHandler(RequestNavigateEvent, value);
            }
        }
    }

    public enum LinkLabelBehavior
    {
        SystemDefault,
        AlwaysUnderline,
        HoverUnderline,
        NeverUnderline
    }

    public class BindableRun : Run
    {
        public static readonly DependencyProperty BoundTextProperty =
            DependencyProperty.Register("BoundText", typeof(string),
            typeof(BindableRun),
            new PropertyMetadata(
                new PropertyChangedCallback(BindableRun.OnBoundTextChanged)));

        private static void OnBoundTextChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((Run)d).Text = e.NewValue as string;
        }

        public string BoundText
        {
            get { return GetValue(BoundTextProperty) as string; }
            set { SetValue(BoundTextProperty, value); }
        }
    }
}
