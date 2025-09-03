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


        public MainWindowViewModel() { }

        

        public MainWindowViewModel(INavigationService navService)
        {
            Navigation = navService;
            Navigation.NavigateTo<UserCampaignsViewModel>();

            NavigateToSignin = new RelayCommand(o =>
            {
                ShowNavigation = false;
                Navigation.NavigateTo<SigninViewModel>();
            });

            GoToStep1 = new RelayCommand(o =>
            {
                ShowNavigation = false;
                Navigation.NavigateTo<FundraisingStepsViewModel>();
            });


        }
    }
}
