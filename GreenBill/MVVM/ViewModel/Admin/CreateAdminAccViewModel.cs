using GreenBill.Core;
using GreenBill.Helpers;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GreenBill.MVVM.ViewModel.Admin {
    internal class CreateAdminAccViewModel: Core.ViewModel {

        private ITabNavigationService _tabNavigationService;
        private BitmapImage _profileImage;
        private readonly IUserService _userService;
        private ITabNavigationService Navigation {
            get => _tabNavigationService;
            set {
                _tabNavigationService = value;
                OnPropertyChanged();
            }
        }

        public string FirstName {
            get => NewUser.FirstName;
            set {
                if (NewUser != null) {
                    NewUser.FirstName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LastName {
            get => NewUser.LastName;
            set {
                if (NewUser != null) {
                    NewUser.LastName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Role {
            get => NewUser.Role;
            set {
                if (NewUser != null) {
                    NewUser.Role = value == "Administrator" ? "admin" : value;
                    OnPropertyChanged();
                }
            }
        }
        



        public string Username {
            get => NewUser?.Username;
            set {
                if (NewUser != null) {
                    NewUser.Username = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Email {
            get => NewUser?.Email;
            set {
                if (NewUser != null) {
                    NewUser.Email = value.ToLower();
                    OnPropertyChanged();
                }
            }
        }

        public string Password {
            get => NewUser?.Password;
            set {
                if (NewUser != null) {
                    NewUser.Password = value;
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



        public User NewUser { get; set;}

        public ICommand NavigateBack { get; set; }
        public ICommand CreateAdminCommand { get; set; }
        public CreateAdminAccViewModel(ITabNavigationService navService, IUserService userService) {
            Navigation = navService;
            _userService = userService;
            NewUser = new User();
            NavigateBack = new RelayCommand(_ => { Navigation.NavigateToTab<AdminAccPreviewViewModel>(); });
            InitializeCommands();
            Role = "Administrator";
        }

        private void LoadProfileImage() {
            if (NewUser?.Profile != null && NewUser.Profile.Length > 0) {
                ProfileImage = ByteArrayToImage(NewUser.Profile);
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
                    NewUser.Profile = imageBytes;

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

        public void InitializeCommands() {

            CreateAdminCommand = new RelayCommand(async (o) => {
                try {
                    IsLoading = true;
                    ValidateInputs();
                    if (HasErrors) {
                        IsLoading = false;
                        return;
                    }

                    await _userService.Create(NewUser);
                    ShowSuccessMessage = true;
                    ResetInputs();

                    Dictionary<string, object> props = new Dictionary<string, object>();
                    props.Add("success", true);
                    props.Add("message", "Account Created Successfully.");
                } catch (Exception ex) {
                    MessageBox.Show($"Error: {ex.Message}");
                } finally {
                    IsLoading = false;
                }
            }, o => NewUser != null);
        }

        public void ResetInputs() {
            NewUser.Username = "";
            NewUser.Email = "";
            NewUser.Password = "";
            NewUser.FirstName = "";
            NewUser.LastName = "";
            NewUser.Role = "";
            Role = "";
            Username = "";
            Email = "";
            FirstName = " ";
            LastName = "";
            Password = "";
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

            if(string.IsNullOrEmpty(NewUser.FirstName)) {
                FirstNameError = "This field is Required";
                HasErrors = true;
            }

            if (string.IsNullOrEmpty(NewUser.LastName)) {
                LastNameError = "This field is Required";
                HasErrors = true;
            }

            if (string.IsNullOrEmpty(NewUser.Username)) {
                UsernameError = "This field is Required";
                HasErrors = true;
            } else if (!Validator.ShouldContainLetter(NewUser.Username)) {
                UsernameError = "Username should contain letters.";
            }

            if (string.IsNullOrEmpty(NewUser.Email)) {
                EmailError = "This field is Required";
                HasErrors = true;
            } else if (!Validator.IsValidEmail(NewUser.Email)) {
                EmailError = "Invalid Email";
                HasErrors = true;
            }


            if (string.IsNullOrEmpty(NewUser.Password)) {
                PasswordError = "This field is Required";
                HasErrors = true;
            } else if (!Validator.IsValidPassword(NewUser.Password)) {
                PasswordError = "Password should be at least 8 characters.";
                HasErrors = true;
            }

            if (string.IsNullOrEmpty(NewUser.Role)) {
                RoleError = "This field is Required";
                HasErrors = true;
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
    }
}
