using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel.Admin {
    internal class AdminAccPreviewViewModel: Core.ViewModel {

        private IUserService _userService;
        private ITabNavigationService _tabNavigationService;
        public ITabNavigationService Navigation {
            get => _tabNavigationService;
            set {
                _tabNavigationService = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();
        
        public ICommand CreateAdminCommand { get; set; }
        public ICommand NavigateToReviewAdmin { get; set; }

        public AdminAccPreviewViewModel(IUserService userService, ITabNavigationService tabNavigationService) {
            _userService = userService;
            Navigation = tabNavigationService;
            _ = LoadUsersAsync();
            CreateAdminCommand = new RelayCommand(_ => { Navigation?.NavigateToTab<CreateAdminAccViewModel>();});
            NavigateToReviewAdmin = new RelayCommand(admin_id => { Navigation?.NavigateToTab<ReviewAdminViewModel>(admin_id);});
        }

        private async Task LoadUsersAsync() {
            try {
                var usersFromDB = await _userService.GetAllUsersAsync();
                if (usersFromDB == null) return;
                foreach (var user in usersFromDB) {
                    if (user.Role == "admin") {
                        Users.Add(user);
                    }
                }
                OnPropertyChanged(nameof(usersFromDB));
            } catch(Exception e) {
                MessageBox.Show("Error retrieving data: " + e);
            }
        }

    }
}
