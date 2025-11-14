using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel.Admin {
    internal class ReviewUserViewModel : Core.ViewModel, INavigatableService {

        private ITabNavigationService _navigationService;
        public ITabNavigationService Navigation {
            get => _navigationService;
            set {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        public IUserService _userService;

        public ICommand NavigateBack { get; set; }
        public ReviewUserViewModel(ITabNavigationService navigationService, IUserService userService) {
            Navigation = navigationService;

            NavigateBack = new RelayCommand(Navigation.NavigateToTab<UserAnalyticsViewModel>);
            _userService = userService;
        }

        private GreenBill.MVVM.Model.User _selectedUser;
        public GreenBill.MVVM.Model.User SelectedUser {
            get => _selectedUser;
            set {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

        public async void ApplyNavigationParameter(object parameter) {
            try {
                if (parameter == null) return;
                string id = parameter.ToString();

                Debug.WriteLine("START TEST");
                SelectedUser = await _userService.GetUserByIdAsync(id); 
                if (SelectedUser == null) throw new ArgumentNullException();
            } catch (Exception ex) {
                MessageBox.Show("Error loading user details: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
