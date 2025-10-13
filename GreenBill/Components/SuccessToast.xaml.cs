using System.Windows;
using System.Windows.Controls;

namespace GreenBill.Components
{
    public partial class SuccessToast : UserControl
    {
        public SuccessToast()
        {
            InitializeComponent();
        }

        // IsVisible Dependency Property
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register(
                "IsVisible",
                typeof(Visibility),
                typeof(SuccessToast),
                new PropertyMetadata(Visibility.Collapsed));

        public Visibility IsVisible
        {
            get { return (Visibility)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        // Title Dependency Property
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title",
                typeof(string),
                typeof(SuccessToast),
                new PropertyMetadata("Success!"));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Message Dependency Property
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                "Message",
                typeof(string),
                typeof(SuccessToast),
                new PropertyMetadata("Operation completed successfully."));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // CloseCommand Dependency Property (optional)
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(
                "CloseCommand",
                typeof(System.Windows.Input.ICommand),
                typeof(SuccessToast),
                new PropertyMetadata(null));

        public System.Windows.Input.ICommand CloseCommand
        {
            get { return (System.Windows.Input.ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        // Close button click handler
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Execute command if provided
            if (CloseCommand != null && CloseCommand.CanExecute(null))
            {
                CloseCommand.Execute(null);
            }

            // Always hide the toast
            IsVisible = Visibility.Collapsed;
        }
    }
}