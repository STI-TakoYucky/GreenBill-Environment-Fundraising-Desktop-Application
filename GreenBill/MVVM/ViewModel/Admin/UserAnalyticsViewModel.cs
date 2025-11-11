using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        private ITabNavigationService _navigationService;

        public ITabNavigationService Navigation {
            get => _navigationService;
            set {
                _navigationService = value;
                OnPropertyChanged();
            }
        }
        public ICommand NavigateToReviewUser { get; set; }

        // Default constructor used by TabNavigationService
        public UserAnalyticsViewModel(ITabNavigationService navigationService) {
            // Create service manually (not via DI)
            _userService = new UserService();
            Navigation = navigationService;
            _ = LoadUsersAsync();

            NavigateToReviewUser = new RelayCommand(userId => Navigation.NavigateToTab<ReviewUserViewModel>(userId.ToString()));
        }

        private async Task LoadUsersAsync() {
            usersFromDB = await _userService.GetAllUsersAsync();
            if (usersFromDB == null) return;

            try {
                UserCount = usersFromDB.Count.ToString();
                foreach (var item in usersFromDB) {
                    Users.Add(new User {
                        Id = item.Id,
                        Profile = item.Profile,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Username = item.Username,
                        Email = item.Email,
                        Password = item.Password,
                        CreatedAt = item.CreatedAt
                    });
                }
                OnPropertyChanged(nameof(usersFromDB));
            } catch (Exception ex) {
                MessageBox.Show($"Error fetching users: {ex.Message}");
            }
        }
    }
}
