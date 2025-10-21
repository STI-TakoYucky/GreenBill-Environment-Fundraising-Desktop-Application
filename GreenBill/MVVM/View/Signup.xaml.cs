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

namespace GreenBill.MVVM.View
{
    /// <summary>
    /// Interaction logic for Signup.xaml
    /// </summary>
    public partial class Signup : UserControl
    {
        private bool isPasswordVisible = false;
        private bool isUpdatingPassword = false;

        public Signup()
        {
            InitializeComponent();
        }


        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (isUpdatingPassword) return;

            var passwordBox = sender as PasswordBox;

            // Update placeholder visibility
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(passwordBox.Password) ?
                Visibility.Visible : Visibility.Collapsed;

            // Sync with TextBox if it's visible
            if (isPasswordVisible)
            {
                isUpdatingPassword = true;
                PasswordTextBox.Text = passwordBox.Password;
                isUpdatingPassword = false;
            }
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingPassword) return;

            var textBox = sender as TextBox;

            // Update placeholder visibility
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(textBox.Text) ?
                Visibility.Visible : Visibility.Collapsed;

            // Sync with PasswordBox
            if (isPasswordVisible)
            {
                isUpdatingPassword = true;
                PasswordBox.Password = textBox.Text;
                isUpdatingPassword = false;
            }
        }

        private void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                // Show password as text
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordTextBox.Focus();
                PasswordTextBox.CaretIndex = PasswordTextBox.Text.Length;

                // Change icon to "hide" icon (eye with slash)
                EyeIcon.Data = Geometry.Parse("M2,5.27L3.28,4L20,20.72L18.73,22L15.65,18.92C14.5,19.3 13.28,19.5 12,19.5C7,19.5 2.73,16.39 1,12C1.69,10.24 2.79,8.69 4.19,7.46L2,5.27M7.53,9.8L9.08,11.35C9.03,11.56 9,11.77 9,12A3,3 0 0,0 12,15C12.22,15 12.44,14.97 12.65,14.92L14.2,16.47C13.53,16.8 12.79,17 12,17A5,5 0 0,1 7,12C7,11.21 7.2,10.47 7.53,9.8M2,4.27L4.28,2L20,17.72L18.73,19L15.65,15.92C14.5,16.3 13.28,16.5 12,16.5C7,16.5 2.73,13.39 1,9C1.69,7.24 2.79,5.69 4.19,4.46L2,2.27M12,5.5C17,5.5 21.27,8.61 23,13C22.18,14.92 20.82,16.58 19.17,17.83L17.58,16.24C18.38,15.55 19.06,14.68 19.56,13.7C18.79,12.34 17.57,11.2 16.07,10.5L14.07,8.5C13.5,8.18 12.78,8 12,8A4,4 0 0,0 8,12C8,12.55 8.1,13.08 8.29,13.56L6.46,11.73C6.88,10.5 7.6,9.39 8.56,8.5C9.56,7.61 10.76,7 12,6.5V5.5Z");
            }
            else
            {
                // Hide password (show as dots)
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordBox.Focus();

                // Change icon back to "show" icon
                EyeIcon.Data = Geometry.Parse("M12,9A3,3 0 0,0 9,12A3,3 0 0,0 12,15A3,3 0 0,0 15,12A3,3 0 0,0 12,9M12,17A5,5 0 0,1 7,12A5,5 0 0,1 12,7A5,5 0 0,1 17,12A5,5 0 0,1 12,17M12,4.5C7,4.5 2.73,7.61 1,12C2.73,16.39 7,19.5 12,19.5C17,19.5 21.27,16.39 23,12C21.27,7.61 17,4.5 12,4.5Z");
            }
        }

        private async void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingOverlay.Visibility = Visibility.Visible;


            await Task.Delay(5000);


            LoadingOverlay.Visibility = Visibility.Collapsed;
        }
    }

}
