using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.MVVM.ViewModel.Admin;
using GreenBill.Services;
using LiveCharts;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GreenBill.MVVM.ViewModel
{
    public class MainWindowViewModel : Core.ViewModel
    {
        private IMongoCollection<User> _collection;
        private IUserService _userService;

        // Tes tcomment zsdfasdfadsfadsf
        private bool _showNavigation = true;
        private bool _isUserLoggedIn;
        private IUserSessionService _sessionService;

        private byte[] _profile;
        public byte[] Profile
        {
            get => _profile;
            set
            {
                _profile = value;
                OnPropertyChanged();
            }
        }

        public bool IsUserLoggedIn
        {
            get => _isUserLoggedIn;
            set
            {
                _isUserLoggedIn = value;
                OnPropertyChanged();
            }
        }

        private bool is_admin = false;
        public bool IsAdmin
        {
            get => is_admin;
            set
            {
                is_admin = value;
                OnPropertyChanged();
            }
        }

        public User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        private INavigationService _navigationService;

        public bool ShowNavigation
        {
            get => _showNavigation;
            set
            {
                _showNavigation = value;
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
        public ICommand NavigateToSignin { get; }

        public ICommand GoToStep1 { get; }
        public ICommand GoToDashboard { get; }
        public ICommand GoToHome { get; }
        public ICommand GoToAdminDashboard { get; }
        public ICommand GoToProfile { get; }

        public ICommand Logout { get; }


        public MainWindowViewModel() { }

        

        public MainWindowViewModel(INavigationService navService, IUserSessionService sessionService, IUserService userService)
        {
            _userService = userService;
            Navigation = navService;
            _sessionService = sessionService;
            Navigation.NavigateTo<HomePageViewModel>();
            NavigateToSignin = new RelayCommand(o => Navigation.NavigateTo<SigninViewModel>());
            GoToStep1 = new RelayCommand(o =>
            {
                CampaignIncludeOptions test = new CampaignIncludeOptions();
                if (_sessionService.IsUserLoggedIn)
                {
                    Navigation.NavigateTo<FundraisingStepsViewModel>();
                }else
                {
                    Navigation.NavigateTo<SigninViewModel>();
                }
            });
            GoToDashboard = new RelayCommand(o => Navigation.NavigateTo<UserCampaignsViewModel>());
            GoToHome = new RelayCommand(o => Navigation.NavigateTo<HomePageViewModel>());
            GoToAdminDashboard = new RelayCommand(o => Navigation.NavigateTo<AdminWindowViewModel>());
            GoToProfile = new RelayCommand(o => Navigation.NavigateTo<MyProfileViewModel>());
            Logout = new RelayCommand(o => _sessionService.ClearSession());

            User currentUser = _sessionService.CurrentUser;
            if(currentUser != null)
            {
                IsAdmin = currentUser.Role.ToLower() == "admin" || currentUser.Role.ToLower() == "superadmin";
                Debug.WriteLine(currentUser.Role);
              
            }
            else
            {
                Debug.WriteLine("No user");
            }

            Debug.WriteLine(IsAdmin);




            defaultLogin();


        }

        public async void defaultLogin()
        {
            _collection = _userService.Collection;
            var filter = Builders<User>.Filter.Eq(u => u.Email, "admin@gmail.com");
            var user = await _collection.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {
                return;
            }


           _sessionService.SetCurrentUser(user);

           ShowNavigation = true;
           IsUserLoggedIn = true;
            Profile = user.Profile;
            IsAdmin = true;


        }

    
    }
}
