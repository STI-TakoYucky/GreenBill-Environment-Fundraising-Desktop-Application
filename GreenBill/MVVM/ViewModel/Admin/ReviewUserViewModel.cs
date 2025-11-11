using GreenBill.Core;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel.Admin {
    internal class ReviewUserViewModel : Core.ViewModel {

        private ITabNavigationService _navigationService;
        public ITabNavigationService Navigation {
            get => _navigationService;
            set {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateBack { get; set; }
        public ReviewUserViewModel(ITabNavigationService navigationService) {
            Navigation = navigationService;

            NavigateBack = new RelayCommand(Navigation.NavigateToTab<UserAnalyticsViewModel>);
        }
    }
}
