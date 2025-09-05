using GreenBill.Core;
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
        private IUserSessionService _sessionService;

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

        public ICommand Logout { get; }


        public MainWindowViewModel() { }

        

        public MainWindowViewModel(INavigationService navService, IUserSessionService sessionService)
        {
            Navigation = navService;
            _sessionService = sessionService;
            Navigation.NavigateTo<HomePageViewModel>();

            NavigateToSignin = new RelayCommand(o => Navigation.NavigateTo<SigninViewModel>());
            GoToStep1 = new RelayCommand(o => Navigation.NavigateTo<FundraisingStepsViewModel>());
            GoToDashboard = new RelayCommand(o => Navigation.NavigateTo<UserCampaignsViewModel>());
            GoToHome = new RelayCommand(o => Navigation.NavigateTo<HomePageViewModel>());
            Logout = new RelayCommand(o => _sessionService.ClearSession());


        }
    }
}
