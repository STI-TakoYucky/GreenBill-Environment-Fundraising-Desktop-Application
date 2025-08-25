using GreenBill.Core;
using GreenBill.MVVM.Model;
using GreenBill.Services;
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
    public class SignupViewModel : Core.ViewModel
    {

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
        public ICommand CreateAccount { get; set; }

        public RelayCommand NavigateToHome { get; set; }

        public SignupViewModel(INavigationService navService, IUserService userService)
        {
            Navigation = navService;
            _userService = userService;
            NewUser = new User();
            NavigateToHome = new RelayCommand(o =>
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainWindowViewModel mainVM)
                {
                    mainVM.ShowNavigation = true;
                }
                Navigation.NavigateTo<HomePageViewModel>();
            });

            CreateAccount = new RelayCommand(o =>
            {
                try
                {
                    _userService.Create(NewUser);
                    MessageBox.Show("Campaign saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    var mainWindow = Application.Current.MainWindow;
                    if (mainWindow?.DataContext is MainWindowViewModel mainVM)
                    {
                        mainVM.ShowNavigation = true;
                    }
                    Navigation?.NavigateTo<HomePageViewModel>();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
               
            }, o => NewUser != null);
        }


    }
}
