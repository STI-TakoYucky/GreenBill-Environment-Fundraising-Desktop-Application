using GreenBill.MVVM.View;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using GreenBill.Core;

namespace GreenBill.MVVM.ViewModel
{
    public class HomePageViewModel : Core.ViewModel
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

        public ICommand NavigateToSignin {  get; set; }
        public HomePageViewModel() 
        {
        }

        public HomePageViewModel(INavigationService navService)
        {
            Navigation = navService;
            NavigateToSignin = new RelayCommand(o => Navigation.NavigateTo<SigninViewModel>());
        }
    }
}
