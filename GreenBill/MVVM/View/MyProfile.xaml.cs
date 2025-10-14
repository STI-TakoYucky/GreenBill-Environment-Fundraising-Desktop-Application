using GreenBill.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GreenBill.MVVM.View
{
    /// <summary>
    /// Interaction logic for MyProfile.xaml
    /// </summary>
    public partial class MyProfile : UserControl
    {
        public MyProfile()
        {
            InitializeComponent();
            ChangePhotoButton.Click += UploadPhotoButton_Click;
        }

        private void UploadPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            // Call the ViewModel's upload method
            if (DataContext is MyProfileViewModel viewModel)
            {
                viewModel.UploadProfilePicture();
            }
        }
    }
}