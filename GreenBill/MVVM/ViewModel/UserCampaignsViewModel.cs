using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel
{
    public class UserCampaignsViewModel : Core.ViewModel, INavigationAware
    {
        private INavigationService _navigationService;
        private ObservableCollection<Campaign> _campaigns;
        private ICampaignService _campaignService;
        public ICommand LoadCampaignsCommand { get; }

        public ObservableCollection<Campaign> Campaigns
        {
            get => _campaigns;
            set
            {
                _campaigns = value;
                OnPropertyChanged();
            }
        }
        public INavigationService Navigation
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        public bool ShowNavigation => true;

        public RelayCommand ViewDetails { get; set; }
        public UserCampaignsViewModel(INavigationService navService, ICampaignService campaignService)
        {
            Navigation = navService;
            _campaignService = campaignService;
            ViewDetails = new RelayCommand(campaign_id => Navigation.NavigateTo<CampaignDetailsViewModel>(campaign_id));
            Campaigns = new ObservableCollection<Campaign>();

            LoadCampaignsCommand = new RelayCommand(async o => await LoadCampaignsAsync());

            _ = LoadCampaignsAsync();
        }

        private async Task LoadCampaignsAsync()
        {
            try
            {
                var campaigns = await _campaignService.GetAllCampaignsAsync();

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
