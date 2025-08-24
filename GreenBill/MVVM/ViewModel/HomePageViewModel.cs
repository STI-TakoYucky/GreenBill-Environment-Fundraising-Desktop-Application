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
using GreenBill.MVVM.Model;
using System.Collections.ObjectModel;
using MongoDB.Driver;

namespace GreenBill.MVVM.ViewModel
{
    public class HomePageViewModel : Core.ViewModel
    {
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

        public HomePageViewModel() 
        {
            Campaigns = new ObservableCollection<Campaign>();
            LoadCampaignsCommand = new RelayCommand(async o => await LoadCampaignsAsync());
            Debug.WriteLine("No parameters");
        }

        public HomePageViewModel(INavigationService navService, ICampaignService campaignService)
        {
            Debug.WriteLine("With parameters");
            Navigation = navService;
            _campaignService = campaignService;
            Campaigns = new ObservableCollection<Campaign>();

            NavigateToFundraisingDetails = new RelayCommand(o =>
            {
                if (o != null)
                {
                    Navigation.NavigateTo<FundraisingDetailsViewModel>(o.ToString());
                }
            });

            LoadCampaignsCommand = new RelayCommand(async o => await LoadCampaignsAsync());

            _ = LoadCampaignsAsync();
        }

        private async Task LoadCampaignsAsync()
        {
            Debug.WriteLine("Load campagins");
            try
            {
                var campaigns = await _campaignService.GetAllCampaignsAsync();

                Campaigns.Clear();
                foreach (var campaign in campaigns)
                {
                    Campaigns.Add(campaign);
                    Debug.WriteLine(campaign.Country);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading campaigns: {ex.Message}");
            }
        }
    }
}
