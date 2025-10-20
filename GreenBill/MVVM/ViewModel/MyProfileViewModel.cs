using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using Microsoft.Win32;
using Stripe.Radar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GreenBill.MVVM.ViewModel
{
    public class MyProfileViewModel : Core.ViewModel
    {
   
        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private bool _showMessage;
        public bool ShowMessage
        {
            get => _showMessage;
            set {
                _showMessage = true;
                OnPropertyChanged();
            }
        }

        private string _successMessage;
        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                _successMessage = value;
                OnPropertyChanged();
            }
        }

        private INavigationService _navigationService;
        private readonly IStripeService _stripeService;
        private readonly IUserSessionService _userSessionService;
        private readonly IUserService _userService;
        private User _currentUser;

        // Properties to track original values for cancel functionality
        private string _originalFirstName;
        private string _originalLastName;
        private string _originalUsername;
        private string _originalEmail;
        private byte[] _originalProfile;

        // Properties for two-way binding
        private string _firstName;
        private string _lastName;
        private string _username;
        private string _email;
        private BitmapImage _profileImage;

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage ProfileImage
        {
            get => _profileImage;
            set
            {
                _profileImage = value;
                OnPropertyChanged();
            }
        }

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                LoadUserData();
            }
        }

        public INavigationService Navigation
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConnectStripeAccountCommand { get; }
        public ICommand UpdateProfileCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BackCommand {  get; }

        public MyProfileViewModel(INavigationService navigationService, IStripeService stripeService,
            IUserSessionService userSessionService, IUserService userService)
        {
            Navigation = navigationService;
            _stripeService = stripeService;
            _userSessionService = userSessionService;
            _userService = userService;
            this.CurrentUser = _userSessionService.CurrentUser;

            ConnectStripeAccountCommand = new RelayCommand(async (o) =>
            {
                await _stripeService.CreateConnectAccountAsync(this.CurrentUser);
                this.CurrentUser = _userSessionService.CurrentUser;
            });

            UpdateProfileCommand = new RelayCommand(async (o) => await UpdateProfile());
            CancelCommand = new RelayCommand(o => CancelChanges());
            BackCommand = new RelayCommand(o => Navigation.NavigateBack(), o => Navigation.CanNavigateBack);
        }

        private void LoadUserData()
        {
            if (CurrentUser != null)
            {
                // Load current values
                FirstName = CurrentUser.FirstName ?? string.Empty;
                LastName = CurrentUser.LastName ?? string.Empty;
                Username = CurrentUser.Username ?? string.Empty;
                Email = CurrentUser.Email ?? string.Empty;

                // Load profile picture
                LoadProfileImage();

                // Store original values for cancel functionality
                _originalFirstName = FirstName;
                _originalLastName = LastName;
                _originalUsername = Username;
                _originalEmail = Email;
                _originalProfile = CurrentUser.Profile;
            }
        }

        private void LoadProfileImage()
        {
            if (CurrentUser?.Profile != null && CurrentUser.Profile.Length > 0)
            {
                ProfileImage = ByteArrayToImage(CurrentUser.Profile);
            }
            else
            {
                // Load default image if no profile picture exists
                try
                {
                    ProfileImage = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/defaultProfile.jpg"));
                }
                catch
                {
                    // If default image doesn't exist, create a blank image
                    ProfileImage = null;
                }
            }
        }

        public void UploadProfilePicture()
        {
            try
            {
                // Open file dialog
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Select Profile Picture",
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*",
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;

                    // Read the file and convert to byte array
                    byte[] imageBytes = File.ReadAllBytes(filePath);

                    // Validate file size (e.g., max 5MB)
                    if (imageBytes.Length > 5 * 1024 * 1024)
                    {
                        MessageBox.Show("Image file size must be less than 5MB.", "File Too Large",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Update the current user's profile picture
                    CurrentUser.Profile = imageBytes;

                    // Update the display
                    LoadProfileImage();

                    MessageBox.Show("Profile picture updated. Click 'Update Profile' to save changes.",
                        "Picture Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading profile picture: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateProfile()
        {
            try
            {
                IsLoading = true;
                // Validate input
                if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                {
                    MessageBox.Show("First Name and Last Name are required.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Username))
                {
                    MessageBox.Show("Username is required.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
                {
                    MessageBox.Show("A valid email address is required.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Update the current user object
                CurrentUser.FirstName = FirstName.Trim();
                CurrentUser.LastName = LastName.Trim();
                CurrentUser.Username = Username.Trim();
                CurrentUser.Email = Email.Trim();

                // Update the user in the database
                await _userService.UpdateUserAsync(CurrentUser.Id, CurrentUser);

                // Update original values to reflect the saved state
                _originalFirstName = FirstName;
                _originalLastName = LastName;
                _originalUsername = Username;
                _originalEmail = Email;
                _originalProfile = CurrentUser.Profile;

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainWindowViewModel mainVM)
                {
                    mainVM.ShowNavigation = true;
                    mainVM.IsUserLoggedIn = true;
                    mainVM.Profile = CurrentUser.Profile;
                }


                ShowMessage = true;
                SuccessMessage = "Profile details updated";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating profile: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CancelChanges()
        {
            // Restore original values
            FirstName = _originalFirstName;
            LastName = _originalLastName;
            Username = _originalUsername;
            Email = _originalEmail;
            CurrentUser.Profile = _originalProfile;

            // Reload the profile image
            LoadProfileImage();

            MessageBox.Show("Changes have been cancelled.", "Cancelled",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private BitmapImage ByteArrayToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}