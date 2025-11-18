using GreenBill.Core;
using GreenBill.Helpers;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.MVVM.View;
using GreenBill.Services;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
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

        public bool _showMessage;
        public bool ShowMessage
        {
            get => _showMessage;
            set
            {
                _showMessage = value;
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

            CreateAccount = new RelayCommand(async (o) =>
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

                    NewUser.Profile = GetDefaultProfilePicture();

                    await _userService.Create(NewUser);
                    ShowSuccessMessage = true;
                    ResetInputs();

                     Dictionary<string, object> props = new Dictionary<string, object>();
                    props.Add("success", true);
                    props.Add("message", "Account Created Successfully.");

                    Navigation.NavigateTo<SigninViewModel>(props);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
                finally
                {
                    IsLoading = false;
                }
            }, o => NewUser != null);
        }

        private byte[] GetDefaultProfilePicture()
        {
            return Convert.FromBase64String(
                "iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAACXBIWXMAAAsTAAALEwEAmpwYAAADfklEQVR4nO2dS2sUQRSFv4kxJhFf+ELSQF5uXLhw4UKEgKB/wI0/wJ07Ff+BLt2JCxeKG18LF4ILQVDwgYqKbyVRo4kmcaTgVmgmM9PT3VV9q6vOBxcGZpKZe++p6qruW7cKJEmSJEmSJEmSJEnyCQUAbcAocBt4ATwH3gLfgWaiv++Bz8Bn4B3wAngCPABuAheBHqAQrZ4ioA04DjwCPpGfz8Bj4BawFbBQcqCJcRB4TzU+ACeAhcjFBgYwGxgB/uDGX+AGsBiwWCTACrJhDOgGLDYEsMBsxqH6qbIVsNgAwHygQhz/gP3km9PAX+L5DqwGLNYHHCU9vgGHyRdLgAnS5RuwBrDYXuA36fMN2Ede2An8Ip9UgQ2AxXqAr+SXb8AOoGIDeBMvgBY9u6KLEfTSAhYCfzARwF1gJfZZUhfSMB2Yk1n/ViSxhzRsqP8boIg9FgHfSceG+ncBFusD/pKej8AKwGLHgD+k6xtwGLBYJ/CCdI0DOwCL7SPbEOp1YBdgsV7gI9n4AGwCLHYQGCcbo8BuwGIHgL9kZwTYB1isB/hAdkaAPYDFuoBXZOsVsBWw2CLgI9l7D6wFLDYP+ET23gCrAIsVgBdk7xnQA1gMYCf5YBjoBiwGsJV8MARsBCwGsI588RDoAywGsIF88QDoBSwGsJ588QNYD1gMYDX54i2wArAYwBLyzctfN3xhCcAi8slzYB5gMYAl5JPnwGzAYgCLyScP678RsBhAO/nkYf03AhYDaCOfPABmABYDWE4+uQvMACxWYGqAhqy5A8wELEb9f0A+uQ3MBixWYKrDSTa9f88CLFZgapqIrLkFFAGLFZgawSIrbgLtgMUKTI3ulCW3gBmAxVrSLUtuADMBi7WkW5ZcB2YBFmtJtyx5ALQDFpsyuFk+uE79MISC3QEMAk9Jl4dMLQDMQx9z6l/5lkxNrLU4BaWUZpouO9cOWGwBU8s7ZcGYUlN/U9oqwGKdwAvS9RJYClisQPaTpF4o5bNSFp0n/YmRRpVqO03+rnQR2IY9upkaHDsdRoG1gMXagYuk5yJT71TsMRe4Q7wmgMuABTo1g9Jk/RGKy+SbOcAt4tEHbAcs1o6Zm4nxFugCLLaI7PdAvgX2Axab/3979x7aVBQGcPy7sVaxQhGhiqCCOkVF1IEgiguFggsHU1wcVHSgf4AOdKAUnbgpDqfiQB04FSsogoiIIqiIDhTFJ/iggPgAFbC+qKNf4IhIaZukuefe5PvB+Zfc9txz7rfTJLf3ngBmAy/Jz0tgNnCB/HwALgIXgUvkYwC4DFwDfpKPX8AN4DpwE/hNPm4DN4E7wCBQB/qBIeAP+fgL/AAGgTvAbeAO+bgL3AMeAI+Ax8AT4Cn5eAY8Bx6Qjxcjn/UmAJ2D2rOTZkmSpGymx2uAXuA9MET6uO0T8JboOW0H+lz/Q5IkKSP1Aa3AP0mSJEmSJEmSJElKwj/R+nJ6vvTXJgAAAABJRU5ErkJggg=="
            );
        }



        public void ResetInputs()
        {
            NewUser.Username = "";
            NewUser.Email = "";
            NewUser.Password = "";
            Username = "";
            Email = "";
            Password = "";
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
            }else if (!Validator.ShouldContainLetter(NewUser.Username)) 
            {
                UsernameError = "Username should contain letters.";
            }

            if (string.IsNullOrEmpty(NewUser.Email))
            {
                EmailError = "This field is Required";
                HasErrors = true;
            }
            else if (!Validator.IsValidEmail(NewUser.Email))
            {
                EmailError = "Invalid Email";
                HasErrors = true;
            }


            if (string.IsNullOrEmpty(NewUser.Password))
            {
                 PasswordError = "This field is Required";
                 HasErrors = true;
            }else if (!Validator.IsValidPassword(NewUser.Password))
            {
                PasswordError = "Password should be at least 8 characters.";
                HasErrors = true;
            }

        }


    }
}
