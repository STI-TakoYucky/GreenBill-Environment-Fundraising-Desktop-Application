using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel.Admin {
    internal class UserAnalyticsViewModel : Core.ViewModel {
        //set variables
        private readonly IUserService _userService;
        public List<User> usersFromDB { get; private set; }
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

        //usercount
        private string _userCount;
        public string UserCount {
            get => _userCount;
            set {
                if (_userCount != value) {
                    _userCount = value;
                    OnPropertyChanged(nameof(UserCount)); // tells WPF to refresh bindings
                }
            }
        }

        //verified user count

        private string _verifiedUserCount;
        public string VerifiedUserCount {
            get => _verifiedUserCount;
            set {
                _verifiedUserCount = value;
                OnPropertyChanged(nameof(VerifiedUserCount));
            }
        }

        //pending user count, this is for stripe
        private string _pendingUserCount;
        public string PendingUserCount {
            get => _pendingUserCount;
            set {
                _pendingUserCount = value;
                OnPropertyChanged(nameof(PendingUserCount));
            }
        }

        private ITabNavigationService _navigationService;

        public ITabNavigationService Navigation {
            get => _navigationService;
            set {
                _navigationService = value;
                OnPropertyChanged();
            }
        }
        public ICommand NavigateToReviewUser { get; set; }
        public ICommand TabSelect { get; set; }
        private string _activeTab = "all";

        public string activeTab {
            get => _activeTab;
            set {
                _activeTab = value;
                OnPropertyChanged();
                FilterTab();
            }
        }

        // Default constructor used by TabNavigationService
        public UserAnalyticsViewModel(ITabNavigationService navigationService) {
            // Create service manually (not via DI)
            _userService = new UserService();
            Navigation = navigationService;
            _ = LoadUsersAsync();

            NavigateToReviewUser = new RelayCommand(userId => Navigation.NavigateToTab<ReviewUserViewModel>(userId.ToString()));
            TabSelect = new RelayCommand(tab => activeTab = tab.ToString().ToLower());
        }

        private void FilterTab() {
            Users.Clear();
            if (activeTab.ToLower() == "all") {
                foreach (var item in usersFromDB) {
                    Users.Add(item);
                }
                return;
            }

            var filteredUsers = usersFromDB.Where(c => string.Equals(c.VerificationStatus.ToLower(), activeTab, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var item in filteredUsers) {
                Users.Add(item);
            }
        }

        private async Task LoadUsersAsync() {
            usersFromDB = await _userService.GetAllUsersAsync();
            if (usersFromDB == null) return;
            Users.Clear();

            try {
                UserCount = usersFromDB.Count.ToString();
                VerifiedUserCount = usersFromDB.Count(item => item.VerificationStatus.ToLower() == "verified").ToString();
                PendingUserCount = usersFromDB.Count(item => item.VerificationStatus.ToLower() == "pending_onboarding").ToString();

                foreach (var item in usersFromDB) {
                    Users.Add(item);
                }

                OnPropertyChanged(nameof(usersFromDB));
                FilterTab();
            } catch (Exception ex) {
                MessageBox.Show($"Error fetching users: {ex.Message}");
            }
        }

        // Properties for tab styling (active/inactive state)
        public bool isAllUsersActive => activeTab == "all";
        public bool isVerifiedUsersActive => activeTab == "verified";
        public bool isPendingUsersActive => activeTab == "pending_onboarding";

        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) {
            base.OnPropertyChanged(propertyName);

            // When SelectedTab changes, notify the tab state properties
            if (propertyName == nameof(activeTab)) {
                OnPropertyChanged(nameof(isAllUsersActive));
                OnPropertyChanged(nameof(isVerifiedUsersActive));
                OnPropertyChanged(nameof(isPendingUsersActive));
            }
        }
    }
}
