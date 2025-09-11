using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.Services;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel
{
    public class CampaignUpdatesViewModel : Core.ViewModel, INavigationAware, INavigatableService
    {
        public bool ShowNavigation => false;

        private INavigationService _navigationService;
        private ObjectId CampaignId { get; set; }
        public ICommand GoBackCommand { get; set; }
        public INavigationService Navigation
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        public CampaignUpdatesViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            GoBackCommand = new RelayCommand(o =>
            {
                Navigation.NavigateBack();
                Debug.WriteLine("TEST BACK");
            }, o => Navigation.CanNavigateBack);
        }

        public async void ApplyNavigationParameter(object parameter)
        {
            if (parameter == null) return;
            CampaignId = (ObjectId)parameter;

        }
    }
}
