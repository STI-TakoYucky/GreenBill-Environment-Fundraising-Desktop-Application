using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace GreenBill.MVVM.ViewModel.Admin {
    internal class UserAnalyticsViewModel : Core.ViewModel {
        private readonly IUserService _userService;
        public List<User> usersFromDB { get; private set; }
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

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

        // Default constructor used by TabNavigationService
        public UserAnalyticsViewModel() {
            // Create service manually (not via DI)
            _userService = new UserService();
            _ = LoadUsersAsync();
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
