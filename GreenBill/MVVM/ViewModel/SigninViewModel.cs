using GreenBill.Core;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public SigninViewModel(INavigationService navService)
        {
            Navigation = navService;
            NavigateToHome = new RelayCommand(o => Navigation.NavigateTo<HomePageViewModel>());
        }   
    }
}
