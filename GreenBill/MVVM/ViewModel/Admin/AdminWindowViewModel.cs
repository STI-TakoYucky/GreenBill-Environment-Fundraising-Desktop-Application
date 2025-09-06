
using GreenBill.Core;
using GreenBill.Services;
using System;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel.Admin {

    public class AdminWindowViewModel : Core.ViewModel {
        private bool _showNavigation = true;
        private bool _isUserLoggedIn;

        public bool IsUserLoggedIn {
            get => _isUserLoggedIn;
            set {
                _isUserLoggedIn = value;
                OnPropertyChanged();
            }
        }

        private INavigationService _navigationService;

        public bool ShowNavigation {
            get => _showNavigation;
            set {
                _showNavigation = value;
                OnPropertyChanged();
            }
        }

        public INavigationService Navigation {
            get => _navigationService;
            set {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateToDashboard { get; }
        public ICommand NavigateToUserAnalytics { get; }
        public ICommand NavigateToAdminCampaignAnalytics { get; }
        public ICommand NavigateToSettings { get; }


        public AdminWindowViewModel() { }



        public AdminWindowViewModel(INavigationService navService) {
            Navigation = navService;
            Navigation.NavigateTo<AdminDashboardViewModel>();
            NavigateToDashboard = new RelayCommand(o => Navigation.NavigateTo<AdminDashboardViewModel>());
            NavigateToUserAnalytics = new RelayCommand(o => Navigation.NavigateTo<UserAnalyticsViewModel>());
            NavigateToAdminCampaignAnalytics = new RelayCommand(o => Navigation.NavigateTo<AdminCampaignAnalyticsViewModel>());
            NavigateToSettings = new RelayCommand(o => Navigation.NavigateTo<SettingsViewModel>());


        }
    }

}
