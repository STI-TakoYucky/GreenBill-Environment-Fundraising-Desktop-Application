using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
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
    public class MyProfileViewModel : Core.ViewModel
    {
        private INavigationService _navigationService;
        private readonly IStripeService _stripeService;
        private readonly IUserSessionService _userSessionService;
        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
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

        public ICommand ConnectStripeAccountCommand { get; }

        public MyProfileViewModel(INavigationService navigationService, IStripeService stripeService, IUserSessionService userSessionService)
        {
            Navigation = navigationService;
            _stripeService = stripeService;
            _userSessionService = userSessionService;
            this.CurrentUser = _userSessionService.CurrentUser;
            ConnectStripeAccountCommand = new RelayCommand(async (o) =>
            {
                await _stripeService.CreateConnectAccountAsync(this.CurrentUser);
            });
            _userSessionService = userSessionService;
        }
    }
}
