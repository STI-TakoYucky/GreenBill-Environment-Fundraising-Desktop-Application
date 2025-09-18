using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel
{
    public class DonationPageViewModel : Core.ViewModel, INavigatableService
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

        public ICommand NavigateBack { get; set; }
        public ICommand CompleteDonation {  get; set; }

        public DonationPageViewModel(INavigationService navService, ICampaignService campaignService)
        {
            Navigation = navService;
            NavigateBack = new RelayCommand(o => Navigation.NavigateBack(), o => Navigation.CanNavigateBack);
            CompleteDonation = new RelayCommand(o => HandleDonation());

        }

        public void HandleDonation()
        {

        }

        public async void ApplyNavigationParameter(object parameter)
        {
            if (parameter == null) return;
            var id = parameter.ToString();
        }
    }
}
