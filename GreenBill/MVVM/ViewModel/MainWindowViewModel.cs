using GreenBill.Core;
using GreenBill.MVVM.ViewModel.Admin;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel
{
    public class MainWindowViewModel : Core.ViewModel
    {
        private bool _showNavigation = true;
        private bool _isUserLoggedIn;

        public bool IsUserLoggedIn
        {
            get => _isUserLoggedIn;
            set
            {
                _isUserLoggedIn = value;
                OnPropertyChanged();
            }
        }

        private INavigationService _navigationService;

        public bool ShowNavigation
        {
            get => _showNavigation;
            set
            {
                _showNavigation = value;
                OnPropertyChanged();
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
        public ICommand NavigateToSignin { get; }

        public ICommand GoToStep1 { get; }
        public ICommand GoToDashboard { get; }
        public ICommand GoToHome { get; }
        public ICommand GoToAdminDashboard { get; }


        public MainWindowViewModel() { }

        

        public MainWindowViewModel(INavigationService navService)
        {
            Navigation = navService;
            Navigation.NavigateTo<AdminWindowViewModel>();

            NavigateToSignin = new RelayCommand(o => Navigation.NavigateTo<SigninViewModel>());
            GoToStep1 = new RelayCommand(o => Navigation.NavigateTo<FundraisingStepsViewModel>());
            GoToDashboard = new RelayCommand(o => Navigation.NavigateTo<UserCampaignsViewModel>());
            GoToHome = new RelayCommand(o => Navigation.NavigateTo<HomePageViewModel>());
            GoToAdminDashboard = new RelayCommand(o => Navigation.NavigateTo<AdminWindowViewModel>());


        }
    }
}
