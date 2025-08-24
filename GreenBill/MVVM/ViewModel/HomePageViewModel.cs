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
using System.Diagnostics;

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

        public ICommand NavigateToFundraisingDetails { get; }

        public HomePageViewModel() 
        {

        }

        public HomePageViewModel(INavigationService navService)
        {
            Navigation = navService;
            NavigateToFundraisingDetails = new RelayCommand(o =>
            {
                Navigation.NavigateTo<FundraisingDetailsViewModel>();
                Debug.WriteLine("CLICKED");
            });
        }
    }
}
