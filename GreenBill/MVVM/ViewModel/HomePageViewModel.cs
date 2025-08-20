using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public HomePageViewModel() { }

        public HomePageViewModel(INavigationService navService)
        {
            Navigation = navService;
        }
    }
}
