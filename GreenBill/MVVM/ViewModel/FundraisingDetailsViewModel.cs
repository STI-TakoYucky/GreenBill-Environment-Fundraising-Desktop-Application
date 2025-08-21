using GreenBill.Core;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
namespace GreenBill.MVVM.ViewModel
{
    public class FundraisingDetailsViewModel : Core.ViewModel
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

        public ICommand NavigateToHome { get; set; }

        public FundraisingDetailsViewModel() {
           
        }

        public FundraisingDetailsViewModel(INavigationService navService)
        {
            Navigation = navService;
            NavigateToHome = new RelayCommand(o => Navigation.NavigateTo<HomePageViewModel>());
        }

    }
}
