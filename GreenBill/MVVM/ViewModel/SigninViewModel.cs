using GreenBill.Core;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GreenBill.IServices;

namespace GreenBill.MVVM.ViewModel
{
    public class SigninViewModel : Core.ViewModel, INavigationAware
    {
        public bool ShowNavigation => false;
        private INavigationService _navigationService;
        private IMongoCollection<User> _collection;
        private IUserService _userService;
        private IUserSessionService _sessionService; 
        private string _errorMessage;
        private bool _isLoading;

        public ICommand Login { get; set; }
        private User NewUser { get; set; }

        public INavigationService Navigation
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => NewUser?.Email;
            set
            {
                if (NewUser != null)
                {
                    NewUser.Email = value;
                    OnPropertyChanged();
                    if (!string.IsNullOrEmpty(ErrorMessage))
                    {
                        ErrorMessage = string.Empty;
                    }
                }
            }
        }

        public string Password
        {
            get => NewUser?.Password;
            set
            {
                if (NewUser != null)
                {
                    NewUser.Password = value;
                    OnPropertyChanged();
                    if (!string.IsNullOrEmpty(ErrorMessage))
                    {
                        ErrorMessage = string.Empty;
                    }
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public SigninViewModel() { }

        public RelayCommand NavigateToHome { get; set; }
        public RelayCommand NavigateToSignup { get; set; }

        public SigninViewModel(INavigationService navService, IUserService userService, IUserSessionService sessionService)
        {
            Navigation = navService;
            _userService = userService;
            _sessionService = sessionService;
            NewUser = new User();


            NavigateToHome = new RelayCommand(o => Navigation.NavigateTo<HomePageViewModel>());

            NavigateToSignup = new RelayCommand(o =>
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainWindowViewModel mainVM)
                {
                    mainVM.ShowNavigation = false;
                }
                Navigation.NavigateTo<SignupViewModel>();
            });

            Login = new RelayCommand(async o =>
            {
                await LoginAsync();
            }, o => CanLogin());
        }

        private bool CanLogin()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private async Task LoginAsync()
        {
          
            if (_userService.Collection  == null)
            {
                ErrorMessage = "Database connection not available.";
                return;
            }

            _collection = _userService.Collection;

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Please enter both email and password.";
                    return;
                }

                var filter = Builders<User>.Filter.Eq(u => u.Email, Email.Trim().ToLowerInvariant());
                var user = await _collection.Find(filter).FirstOrDefaultAsync();

                if (user == null)
                {
                    ErrorMessage = "Invalid email or password.";
                    Debug.WriteLine(ErrorMessage);
                    return;
                }

                if (!VerifyPassword(Password, user.Password))
                {
                    ErrorMessage = "Invalid email or password.";
                    return;
                }

                _sessionService.SetCurrentUser(user);

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainWindowViewModel mainVM)
                {
                    mainVM.ShowNavigation = true;
                    mainVM.IsUserLoggedIn = true;
                }

                Navigation.NavigateTo<HomePageViewModel>();

                Debug.WriteLine("Current User: " + _sessionService.CurrentUser.Username);

                ClearForm();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during login. Please try again.";
                Debug.WriteLine($"Login error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            return enteredPassword == storedPassword;
        }

        private void ClearForm()
        {
            if (NewUser != null)
            {
                NewUser.Email = string.Empty;
                NewUser.Password = string.Empty;
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(Password));
            }
            ErrorMessage = string.Empty;
        }
    }
}