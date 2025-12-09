using GreenBill.Core;
using GreenBill.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel.Admin {
    public class AdminWindowViewModel : Core.ViewModel {
        private bool _showNavigation = true;
        private bool _isUserLoggedIn;
        private string role;
        public string Role { 
            get => role;
            set {
                role = value;
                OnPropertyChanged();
            }
        }

        public bool IsUserLoggedIn {
            get => _isUserLoggedIn;
            set {
                _isUserLoggedIn = value;
                OnPropertyChanged();
            }
        }

        private ITabNavigationService _navigationService;
        private IUserSessionService userSessionService;
        public IUserSessionService UserSessionService {
            get => userSessionService;
            set {
                userSessionService = value;
                OnPropertyChanged();
            }
        }

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
        public ICommand NavigateToAdminAccPreview { get; }

        public ICommand NavigateToReviewCampaign { get; }

        public AdminWindowViewModel() { }

        public AdminWindowViewModel(ITabNavigationService navService, IUserSessionService userSessionService) {
            Navigation = navService;
            UserSessionService = userSessionService;
            Role = userSessionService.CurrentUser.Role;
            Navigation.NavigateToTab<AdminDashboardViewModel>();
            NavigateToDashboard = new RelayCommand(o => Navigation.NavigateToTab<AdminDashboardViewModel>());
            NavigateToUserAnalytics = new RelayCommand(o => Navigation.NavigateToTab<UserAnalyticsViewModel>());
            NavigateToSettings = new RelayCommand(o => Navigation.NavigateToTab<SettingsViewModel>());
            NavigateToAdminCampaignAnalytics = new RelayCommand(o => Navigation.NavigateToTab<AdminCampaignAnalyticsViewModel>());
            NavigateToAdminAccPreview = new RelayCommand(o => Navigation.NavigateToTab<AdminAccPreviewViewModel>());
        }
    }
}
