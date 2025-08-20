using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.MVVM.ViewModel
{
    public class SignupViewModel : Core.ViewModel
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

        public SignupViewModel() { }

        public SignupViewModel(INavigationService navService)
        {
            Navigation = navService;
        }
    }
}
