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
using GreenBill.MVVM.Model;
namespace GreenBill.MVVM.ViewModel
{
    public class FundraisingDetailsViewModel : Core.ViewModel, INavigatableService
    {
        private INavigationService _navigationService;
        private Campaign _selectedCampaign;
        private string _campaignId;
        private ICampaignService _campaignService;

        public INavigationService Navigation
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        public string CampaignId
        {
            get => _campaignId;
            set
            {
                _campaignId = value;
                OnPropertyChanged();
            }
        }

        public Campaign SelectedCampaign
        {
            get => _selectedCampaign;
            set
            {
                _selectedCampaign = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateToHome { get; set; }

        public FundraisingDetailsViewModel() {
           
        }

        public FundraisingDetailsViewModel(INavigationService navService, ICampaignService campaignService)
        {
            Navigation = navService;
            _campaignService = campaignService;
            NavigateToHome = new RelayCommand(o => Navigation.NavigateTo<HomePageViewModel>());
        }
        public async void ApplyNavigationParameter(object parameter)
        {
            if (parameter == null) return;

            var id = parameter.ToString();

            SelectedCampaign = await _campaignService.GetCampaignByIdAsync(id);

            Debug.WriteLine($"Loaded campaign: {SelectedCampaign?.Title}");
        }



    }
}
