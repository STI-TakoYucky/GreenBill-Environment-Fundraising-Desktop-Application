using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel
{
    public class MyProfileViewModel : Core.ViewModel
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

        public ICommand ConnectStripAccountCommand { get; }

        public MyProfileViewModel(INavigationService navigationService)
        {

        }
    }
}
