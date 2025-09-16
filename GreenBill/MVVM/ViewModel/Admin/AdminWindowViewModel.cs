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

        private ITabNavigationService _navigationService;

        public bool ShowNavigation {
            get => _showNavigation;
            set {
                _showNavigation = value;
                OnPropertyChanged();
            }
        }

        public ITabNavigationService Navigation {
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

        public ICommand NavigateToReviewCampaign { get; }
        public AdminWindowViewModel() { }

        public AdminWindowViewModel(ITabNavigationService navService) {
            Navigation = navService;
            Navigation.NavigateToTab<AdminDashboardViewModel>();

            NavigateToDashboard = new RelayCommand(o => Navigation.NavigateToTab<AdminDashboardViewModel>());
            NavigateToUserAnalytics = new RelayCommand(o => Navigation.NavigateToTab<UserAnalyticsViewModel>());
            NavigateToAdminCampaignAnalytics = new RelayCommand(o => Navigation.NavigateToTab<AdminCampaignAnalyticsViewModel>());
            NavigateToSettings = new RelayCommand(o => Navigation.NavigateToTab<SettingsViewModel>());

            NavigateToReviewCampaign = new RelayCommand(o => Navigation.NavigateToTab<ReviewCampaignViewModel>());
        }
    }
}
