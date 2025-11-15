using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.Services;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GreenBill.MVVM.ViewModel.Admin {
    internal class ReviewUserViewModel : Core.ViewModel, INavigatableService {

        private ITabNavigationService _navigationService;
        public ITabNavigationService Navigation {
            get => _navigationService;
            set {
                _navigationService = value;
                OnPropertyChanged();
            }
        }


        private GreenBill.MVVM.Model.User _selectedUser;
        public GreenBill.MVVM.Model.User SelectedUser {
            get => _selectedUser;
            set {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Passwords {
            get => _password;
            set {
                _password = value;
                OnPropertyChanged();
            }
        }

        private bool isPasswordHidden = true;
        private string storedPassword;


        public IUserService _userService;

        public ICommand NavigateBack { get; set; }
        public ICommand PreviewImageCommand { get; }
        public ICommand ShowPasswordCommand { get; }
        public ReviewUserViewModel(ITabNavigationService navigationService, IUserService userService) {
            Navigation = navigationService;

            NavigateBack = new RelayCommand(Navigation.NavigateToTab<UserAnalyticsViewModel>);
            _userService = userService;
            PreviewImageCommand = new RelayCommand(param => PreviewImage(param));
            ShowPasswordCommand = new RelayCommand(_ => ShowPassword());
        }

        private void ShowPassword() {
            int passwordLength = storedPassword.Length;
            Passwords = "";
            if(isPasswordHidden) {
                Passwords = storedPassword;
                isPasswordHidden = false;
            } else if (!isPasswordHidden) {
                for (int i = 1; i <= passwordLength; i++) {
                    Passwords += "•";
                }
                isPasswordHidden = true;
            }
        }

        public void PreviewImage(object parameter) {
            if (parameter is byte[] imageBytes) {
                // Convert byte array to ImageSource
                var image = new BitmapImage();
                using (var stream = new MemoryStream(imageBytes)) {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    image.Freeze(); // optional, makes it cross-thread safe
                }

                var window = new Window {
                    Title = SelectedUser?.Username ?? "Preview",
                    Width = 800,
                    Height = 600,
                    Content = new Image {
                        Source = image,
                        Stretch = Stretch.Uniform
                    }
                };
                window.ShowDialog();
            }
        }

        public async void ApplyNavigationParameter(object parameter) {
            try {
                if (parameter == null) return;
                string id = parameter.ToString();
                SelectedUser = await _userService.GetUserByIdAsync(id); 
                if (SelectedUser == null) throw new ArgumentNullException();

                int passwordLength = SelectedUser.Password.Length;
                for (int i = 1; i <= passwordLength; i++) {
                    Passwords += "•";
                }
                storedPassword = SelectedUser.Password;
            } catch (Exception ex) {
                MessageBox.Show("Error loading user details: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
