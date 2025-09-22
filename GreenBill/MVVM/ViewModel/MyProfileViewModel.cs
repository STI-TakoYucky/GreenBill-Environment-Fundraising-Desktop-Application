using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace GreenBill.MVVM.ViewModel
{
    public class MyProfileViewModel : Core.ViewModel
    {
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

        // Properties for two-way binding
        private string _firstName;
        private string _lastName;
        private string _username;
        private string _email;

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

        public MyProfileViewModel(INavigationService navigationService, IStripeService stripeService, IUserSessionService userSessionService, IUserService userService)
        {
            Navigation = navigationService;
            _stripeService = stripeService;
            _userSessionService = userSessionService;
            this.CurrentUser = _userSessionService.CurrentUser;

            ConnectStripeAccountCommand = new RelayCommand(async (o) =>
            {
                await _stripeService.CreateConnectAccountAsync(this.CurrentUser);
                this.CurrentUser = _userSessionService.CurrentUser;
            });

            UpdateProfileCommand = new RelayCommand(async (o) => await UpdateProfile());
            CancelCommand = new RelayCommand(o => CancelChanges());
            _userService = userService;
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

                // Store original values for cancel functionality
                _originalFirstName = FirstName;
                _originalLastName = LastName;
                _originalUsername = Username;
                _originalEmail = Email;
            }
        }

        private async Task UpdateProfile()
        {
            try
            {
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

                // Here you would typically call a service to update the user in the database
                // For example: await _userService.UpdateUserAsync(CurrentUser);

                // Update the session
                await _userService.UpdateUserAsync(CurrentUser.Id, CurrentUser);

                // Update original values to reflect the saved state
                _originalFirstName = FirstName;
                _originalLastName = LastName;
                _originalUsername = Username;
                _originalEmail = Email;

                MessageBox.Show("Profile updated successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating profile: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelChanges()
        {
            // Restore original values
            FirstName = _originalFirstName;
            LastName = _originalLastName;
            Username = _originalUsername;
            Email = _originalEmail;

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
    }
}