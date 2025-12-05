using GreenBill.Core;
using GreenBill.Helpers;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using Microsoft.Win32;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GreenBill.MVVM.ViewModel.Admin {
    internal class ReviewAdminViewModel : Core.ViewModel, INavigatableService {

        private ITabNavigationService _tabNavigationService;
        private readonly IUserService _userService;
        private BitmapImage _profileImage;

        private User _user;
        public User User {
            get => _user;
            set {
                _user = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FirstName));
                OnPropertyChanged(nameof(LastName));
                OnPropertyChanged(nameof(Role));
                OnPropertyChanged(nameof(Username));
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(Password));
            }
        }

        private ITabNavigationService Navigation {
            get => _tabNavigationService;
            set {
                _tabNavigationService = value;
                OnPropertyChanged();
            }
        }

        public string FirstName {
            get => User.FirstName;
            set {
                if (User != null) {
                    User.FirstName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LastName {
            get => User.LastName;
            set {
                if (User != null) {
                    User.LastName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Role {
            get => User.Role;
            set {
                if (User != null) {
                    User.Role = value == "Administrator" ? "admin" : value;
                    OnPropertyChanged();
                }
            }
        }


        public string Username {
            get => User?.Username;
            set {
                if (User != null) {
                    User.Username = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Email {
            get => User?.Email;
            set {
                if (User != null) {
                    User.Email = value.ToLower();
                    OnPropertyChanged();
                }
            }
        }

        public string Password {
            get => User?.Password;
            set {
                if (User != null) {
                    User.Password = value;
                    OnPropertyChanged();
                }
            }
        }

        public BitmapImage ProfileImage {
            get => _profileImage;
            set {
                _profileImage = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading = false;
        public bool IsLoading {
            get => _isLoading;
            set {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private bool _hasErrors;
        public bool HasErrors {
            get => _hasErrors;
            set {
                _hasErrors = value;
                OnPropertyChanged();
            }
        }

        private bool _showSuccessMessage;
        public bool ShowSuccessMessage {
            get => _showSuccessMessage;
            set {
                _showSuccessMessage = value;
                OnPropertyChanged();
            }
        }

        private string _firstNameError;
        public string FirstNameError {
            get => _firstNameError;
            set {
                _firstNameError = value;
                OnPropertyChanged();
            }
        }

        private string _lastNameError;
        public string LastNameError {
            get => _lastNameError;
            set {
                _lastNameError = value;
                OnPropertyChanged();
            }
        }

        private string _usernameError;
        public string UsernameError {
            get => _usernameError;
            set {
                _usernameError = value;
                OnPropertyChanged();
            }
        }


        private string _emailError;
        public string EmailError {
            get => _emailError;
            set {
                _emailError = value;
                OnPropertyChanged();
            }
        }

        private string _passwordError;
        public string PasswordError {
            get => _passwordError;
            set {
                _passwordError = value;
                OnPropertyChanged();
            }
        }

        private string _roleError;
        public string RoleError {
            get => _roleError;
            set {
                _roleError = value;
                OnPropertyChanged();
            }
        }


        public ICommand UpdateUserCommand { get; set; }
        public ICommand DeleteUserCommand { get; set; }
        public ICommand NavigateBack { get; set; }

        public ReviewAdminViewModel(IUserService userService, ITabNavigationService tabNavigationService) {
            _userService = userService;
            _tabNavigationService = tabNavigationService;
            User = new User();

            UpdateUserCommand = new RelayCommand(_ => updateUserByID(User.Id, User));
            DeleteUserCommand = new RelayCommand(_ => deleteUserById(User.Id));
            NavigateBack = new RelayCommand(_ => Navigation.NavigateToTab<AdminAccPreviewViewModel>());
        }

        public async void fetchUserByID(string id) {
            try {
                User admin = await _userService.GetUserByIdAsync(id);
                User = admin;
                LoadProfileImage();
            }catch (Exception e) {
                MessageBox.Show("Failed to get admin " + e);
            }
        }

        public async void deleteUserById(ObjectId id) {
            try {
                MessageBoxResult result = MessageBox.Show("Confirm Delete?", "Delete Admin", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes) {
                    var admin = await _userService.DeleteUserAsync(id);
                    MessageBox.Show("Admin deleted successfully");
                    Navigation.NavigateToTab<AdminAccPreviewViewModel>();
                }
                return;
            } catch (Exception e) {
                MessageBox.Show("Failed to delete admin " + e);
            }
        }

        public async void updateUserByID(ObjectId id, User user) {
            try {
                ValidateInputs();
                if (HasErrors) return;
                _ = await _userService.UpdateUserAsync(id, User);
                MessageBox.Show("Admin updated successfully");
            }catch(Exception e) {
                MessageBox.Show("Failed to update admin " + e);
            }
        }

        private void LoadProfileImage() {
            if (User?.Profile != null && User.Profile.Length > 0) {
                ProfileImage = ByteArrayToImage(User.Profile);
            } else {
                // Load default image if no profile picture exists
                try {
                    ProfileImage = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/defaultProfile.jpg"));
                } catch {
                    // If default image doesn't exist, create a blank image
                    ProfileImage = null;
                }
            }
        }

        public void UploadProfilePicture() {
            try {
                // Open file dialog
                OpenFileDialog openFileDialog = new OpenFileDialog {
                    Title = "Select Profile Picture",
                    Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*",
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() == true) {
                    string filePath = openFileDialog.FileName;

                    // Read the file and convert to byte array
                    byte[] imageBytes = File.ReadAllBytes(filePath);

                    // Validate file size (e.g., max 5MB)
                    if (imageBytes.Length > 5 * 1024 * 1024) {
                        MessageBox.Show("Image file size must be less than 5MB.", "File Too Large",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Update the current user's profile picture
                    User.Profile = imageBytes;

                    // Update the display
                    LoadProfileImage();

                    MessageBox.Show("Profile picture updated. Click 'Update Profile' to save changes.",
                        "Picture Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            } catch (Exception ex) {
                MessageBox.Show($"Error uploading profile picture: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private BitmapImage ByteArrayToImage(byte[] imageData) {
            if (imageData == null || imageData.Length == 0)
                return null;

            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData)) {
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

        public void ValidateInputs() {
            ShowSuccessMessage = false;
            HasErrors = false;
            UsernameError = "";
            EmailError = "";
            PasswordError = "";
            LastNameError = "";
            RoleError = "";
            FirstNameError = "";

            if (string.IsNullOrEmpty(User.FirstName)) {
                FirstNameError = "This field is Required";
                HasErrors = true;
            }

            if (string.IsNullOrEmpty(User.LastName)) {
                LastNameError = "This field is Required";
                HasErrors = true;
            }

            if (string.IsNullOrEmpty(User.Username)) {
                UsernameError = "This field is Required";
                HasErrors = true;
            } else if (!Validator.ShouldContainLetter(User.Username)) {
                UsernameError = "Username should contain letters.";
            }

            if (string.IsNullOrEmpty(User.Email)) {
                EmailError = "This field is Required";
                HasErrors = true;
            } else if (!Validator.IsValidEmail(User.Email)) {
                EmailError = "Invalid Email";
                HasErrors = true;
            }


            if (string.IsNullOrEmpty(User.Password)) {
                PasswordError = "This field is Required";
                HasErrors = true;
            } else if (!Validator.IsValidPassword(User.Password)) {
                PasswordError = "Password should be at least 8 characters.";
                HasErrors = true;
            }

            if (string.IsNullOrEmpty(User.Role)) {
                RoleError = "This field is Required";
                HasErrors = true;
            }

        }

        public void ApplyNavigationParameter(object parameter) {
            fetchUserByID(parameter.ToString());
        }
    }
}
