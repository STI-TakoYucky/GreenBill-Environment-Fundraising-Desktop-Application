using System.Windows;
using System.Windows.Controls;

namespace GreenBill.Components
{
    public partial class LoadingOverlay : UserControl
    {
        public LoadingOverlay()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register(
                "IsVisible",
                typeof(Visibility),
                typeof(LoadingOverlay),
                new PropertyMetadata(Visibility.Collapsed));

        public Visibility IsVisible
        {
            get { return (Visibility)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public static readonly DependencyProperty LoadingTextProperty =
            DependencyProperty.Register(
                "LoadingText",
                typeof(string),
                typeof(LoadingOverlay),
                new PropertyMetadata("Loading..."));

        public string LoadingText
        {
            get { return (string)GetValue(LoadingTextProperty); }
            set { SetValue(LoadingTextProperty, value); }
        }
    }
}