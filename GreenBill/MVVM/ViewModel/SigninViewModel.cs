using GreenBill.Core;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GreenBill.MVVM.ViewModel
{
    public class SigninViewModel : Core.ViewModel
    {
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

        public SigninViewModel() { }

        public RelayCommand NavigateToHome {  get; set; }
        public RelayCommand NavigateToSignup { get; set; }

        public SigninViewModel(INavigationService navService)
        {
            Navigation = navService;
            
            NavigateToHome = new RelayCommand(o =>
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainWindowViewModel mainVM)
                {
                    mainVM.ShowNavigation = true;
                }
                Navigation.NavigateTo<HomePageViewModel>();
            });

            NavigateToSignup = new RelayCommand(o =>
            {
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow?.DataContext is MainWindowViewModel mainVM)
                {
                    mainVM.ShowNavigation = false;
                }
                Navigation.NavigateTo<SignupViewModel>();
            });
        }   
    }
}
