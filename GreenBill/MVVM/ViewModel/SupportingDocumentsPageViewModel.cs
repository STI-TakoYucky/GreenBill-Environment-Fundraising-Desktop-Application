using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel
{
    public class SupportingDocumentsPageViewModel : Core.ViewModel, INavigationAware
    {
        public bool ShowNavigation => false;

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

        public ICommand NavigateBack { get; set; }

        public SupportingDocumentsPageViewModel(INavigationService navigation)
        {
            Navigation = navigation;
            NavigateBack = new RelayCommand(o => Navigation.NavigateBack(), o => Navigation.CanNavigateBack);
        }
    }
}
