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

namespace GreenBill.MVVM.ViewModel
{
    public class SigninViewModel : Core.ViewModel, INavigationAware
    {
        public bool ShowNavigation => false;
        private INavigationService _navigationService;
        private IMongoCollection<User> _collection;
        private IUserService _userService;
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
                    // Clear error when user starts typing
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
                    // Clear error when user starts typing
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

        public SigninViewModel(INavigationService navService, IUserService userService)
        {
            Navigation = navService;
            _userService = userService;
            NewUser = new User();

            // Initialize MongoDB connection
            InitializeDatabase();

            NavigateToHome = new RelayCommand(o =>
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainWindowViewModel mainVM)
                {
                    mainVM.ShowNavigation = true;
                }
                Navigation.NavigateTo<HomePageViewModel>();
            });

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

        private void InitializeDatabase()
        {
            try
            {
                string connectionString = "mongodb://localhost:27017";
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase("GreenBill");
                _collection = database.GetCollection<User>("Users");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Database connection failed. Please check if MongoDB is running.";
                Debug.WriteLine($"Database initialization error: {ex.Message}");
            }
        }

        private bool CanLogin()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private async Task LoginAsync()
        {
            if (_collection == null)
            {
                ErrorMessage = "Database connection not available.";
                return;
            }

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                // Validate input
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Please enter both email and password.";
                    return;
                }

                // Find user by email
                var filter = Builders<User>.Filter.Eq(u => u.Email, Email.Trim().ToLowerInvariant());
                var user = await _collection.Find(filter).FirstOrDefaultAsync();

                if (user == null)
                {
                    ErrorMessage = "Invalid email or password.";
                    Debug.WriteLine(ErrorMessage);
                    return;
                }

                // Verify password (assuming you have password hashing)
                if (!VerifyPassword(Password, user.Password))
                {
                    ErrorMessage = "Invalid email or password.";
                    return;
                }

                // Login successful - store user session

                // Navigate to home page
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainWindowViewModel mainVM)
                {
                    mainVM.ShowNavigation = true;
                }

                Navigation.NavigateTo<HomePageViewModel>();

                // Clear form
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
            // If passwords are stored as plain text (NOT RECOMMENDED for production)
            // return enteredPassword == storedPassword;

            // If using password hashing (RECOMMENDED)
            // You should use a proper password hashing library like BCrypt
            // Example with BCrypt:
            // return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPassword);

            // For now, using simple comparison (change this in production!)
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