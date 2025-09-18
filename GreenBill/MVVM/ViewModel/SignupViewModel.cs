using GreenBill.Core;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using System.Windows.Input;
using GreenBill.IServices;


namespace GreenBill.MVVM.ViewModel
{
    public class SignupViewModel : Core.ViewModel, INavigationAware
    {
        public bool ShowNavigation => false;
        private IUserService _userService;
        private INavigationService _navigationService;
        public INavigationService Navigation
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        private User NewUser { get; set; }
        public string Username
        {
            get => NewUser?.Username;
            set
            {
                if (NewUser != null)
                {
                    NewUser.Username = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Email
        {
            get => NewUser?.Email;
            set
            {
                if (NewUser != null)
                {
                    NewUser.Email = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Password
        {
            get => NewUser?.Password;
            set
            {
                if (NewUser != null)
                {
                    NewUser.Password = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand CreateAccount { get; set; }

        public RelayCommand NavigateToHome { get; set; }

        public SignupViewModel(INavigationService navService, IUserService userService)
        {
            Navigation = navService;
            _userService = userService;
            NewUser = new User();
            InitializeCommands();
        }


        public void InitializeCommands()
        {
            NavigateToHome = new RelayCommand(o => Navigation.NavigateTo<HomePageViewModel>());

            CreateAccount =  new RelayCommand(async (o)=>
            {
                await _userService.Create(NewUser);
            }, o => NewUser != null);
        }


    }
}
