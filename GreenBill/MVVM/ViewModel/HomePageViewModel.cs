using GreenBill.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GreenBill.Core;
using System.Diagnostics;
using GreenBill.MVVM.Model;
using System.Collections.ObjectModel;
using GreenBill.IServices;

namespace GreenBill.MVVM.ViewModel
{
    public class HomePageViewModel : Core.ViewModel, INavigationAware
    {
        public bool ShowNavigation => true;
        private INavigationService _navigationService;
        private ObservableCollection<Campaign> _campaigns;
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

        public ObservableCollection<Campaign> Campaigns
        {
            get => _campaigns;
            set
            {
                _campaigns = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateToFundraisingDetails { get; }
        public ICommand LoadCampaignsCommand { get; }
        public ICommand ViewMore { get; }

        public HomePageViewModel(INavigationService navService, ICampaignService campaignService)
        {
            Navigation = navService;
            _campaignService = campaignService;
            Campaigns = new ObservableCollection<Campaign>();

            NavigateToFundraisingDetails = new RelayCommand(campaign_id =>
            {
                if (campaign_id != null)
                {
                    Navigation.NavigateTo<FundraisingDetailsViewModel>(campaign_id.ToString());
                }
            });

            ViewMore = new RelayCommand(o => Navigation.NavigateTo<CampaignsViewModel>());

            LoadCampaignsCommand = new RelayCommand(async o => await LoadCampaignsAsync());

            _ = LoadCampaignsAsync();
        }

        private async Task LoadCampaignsAsync()
        {
            try
            {
                var campaigns = await _campaignService.GetAllCampaignsAsync(new CampaignIncludeOptions { IncludeDonationRecord = true});

                Campaigns.Clear();
                foreach (var campaign in campaigns)
                {
                    Campaigns.Add(campaign);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading campaigns: {ex.Message}");
            }
        }
    }
}
