using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using System;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Input;


namespace GreenBill.MVVM.ViewModel
{
    public class SignupViewModel : Core.ViewModel, INavigationAware
    {
        public bool ShowNavigation => false;
        private IUserService _userService;
        private INavigationService _navigationService;
        public INavigationService Navigation
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        private bool _showSuccessMessage = false;
        public bool ShowSuccessMessage
        {
            get => _showSuccessMessage;
            set
            {
                _showSuccessMessage = value;
                OnPropertyChanged();
            }
        }

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

        private bool _hasErrors = false;
        public bool HasErrors
        {
            get => _hasErrors;
            set
            {
                _hasErrors = value;
                OnPropertyChanged();
            }
        }

        private User NewUser { get; set; }
        public string Username
        {
            get => NewUser?.Username;
            set
            {
                if (NewUser != null)
                {
                    NewUser.Username = value;
                    OnPropertyChanged();
                }
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
                }
            }
        }


        private string _usernameError;
        public string UsernameError
        {
            get => _usernameError;
            set  {
                _usernameError = value;
                OnPropertyChanged();
            }
        }
        private string _emailError;
        public string EmailError
        {
            get => _emailError;
            set
            {
                _emailError = value;
                OnPropertyChanged();
            }
        }
        private string _passwordError;
        public string PasswordError
        {
            get => _passwordError;
            set
            {
                _passwordError = value;
                OnPropertyChanged();
            }
        }
        public ICommand CreateAccount { get; set; }
        public ICommand CloseToast { get; set; }

        public RelayCommand NavigateToHome { get; set; }

        public SignupViewModel(INavigationService navService, IUserService userService)
        {
            Navigation = navService;
            _userService = userService;
            NewUser = new User();
            InitializeCommands();
        }


        public void InitializeCommands()
        {
            CloseToast = new RelayCommand(o => ShowSuccessMessage = false);
            NavigateToHome = new RelayCommand(o => Navigation.NavigateTo<HomePageViewModel>());

            CreateAccount =  new RelayCommand(async (o)=>
            {
                try
                {
                    IsLoading = true;
                    ValidateInputs();
                    if (HasErrors)
                    {
                        IsLoading = false;
                        return;
                    }

                    await _userService.Create(NewUser);
                    ShowSuccessMessage = true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
                finally
                {
                    IsLoading = false;
                }
            }, o => NewUser != null);
        }

        public void ValidateInputs()
        {
            ShowSuccessMessage = false;
            HasErrors = false;
            UsernameError = "";
            EmailError = "";
            PasswordError = "";
            if (string.IsNullOrEmpty(NewUser.Username))
            {
                UsernameError = "This field is Required";
                HasErrors = true;
            }
            if (string.IsNullOrEmpty(NewUser.Email))
            {
                EmailError = "This field is Required";
                HasErrors = true;
            }
            if (string.IsNullOrEmpty(NewUser.Password))
            {
                PasswordError = "This field is Required";
                HasErrors = true;
            }

        }


    }
}
