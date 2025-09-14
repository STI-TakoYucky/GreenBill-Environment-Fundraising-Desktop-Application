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
    public class CampaignsViewModel : Core.ViewModel
    {
        private ObservableCollection<Campaign> _campaigns;
        private ObservableCollection<Campaign> _filteredCampaigns;
        private ObservableCollection<Campaign> _allCampaigns;
        private INavigationService _navigationService;
        private ICampaignService _campaignService;
        private string _searchText = string.Empty;
        private string _currentFilter = "trending";

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

        public ObservableCollection<Campaign> FilteredCampaigns
        {
            get => _filteredCampaigns;
            set
            {
                _filteredCampaigns = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplySearchAndFilter();
            }
        }

        public string CurrentFilter
        {
            get => _currentFilter;
            set
            {
                _currentFilter = value;
                OnPropertyChanged();
                ApplySearchAndFilter();
            }
        }

        public ICommand LoadCampaignsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ApplyFilterCommand { get; }
        public ICommand NavigateToCampaignDetails { get; }

        public CampaignsViewModel(INavigationService navService, ICampaignService campaignService)
        {
            Navigation = navService;
            _campaignService = campaignService;

            _allCampaigns = new ObservableCollection<Campaign>();
            Campaigns = new ObservableCollection<Campaign>();
            FilteredCampaigns = new ObservableCollection<Campaign>();

            LoadCampaignsCommand = new RelayCommand(async o => await LoadCampaignsAsync());
            SearchCommand = new RelayCommand(o => PerformSearch(o?.ToString()));
            ApplyFilterCommand = new RelayCommand(o => ApplyFilter(o?.ToString()));

            NavigateToCampaignDetails = new RelayCommand(campaign_id => Navigation.NavigateTo<FundraisingDetailsViewModel>(campaign_id.ToString()));
            _ = LoadCampaignsAsync();
        }

        private async Task LoadCampaignsAsync()
        {
            try
            {
                var campaigns = await _campaignService.GetAllCampaignsAsync();
                _allCampaigns.Clear();
                Campaigns.Clear();

                foreach (var campaign in campaigns)
                {
                    _allCampaigns.Add(campaign);
                    Campaigns.Add(campaign);
                }

                ApplySearchAndFilter();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading campaigns: {ex.Message}");
            }
        }

        private void PerformSearch(string searchText)
        {
            SearchText = searchText ?? string.Empty;
        }

        private void ApplyFilter(string filterType)
        {
            CurrentFilter = filterType?.ToLower() ?? "trending";
        }



        private void ApplySearchAndFilter()
        {
            var filteredList = _allCampaigns.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string searchLower = SearchText.ToLower();
                filteredList = filteredList.Where(campaign =>
                    (!string.IsNullOrEmpty(campaign.Title) && campaign.Title.ToLower().Contains(searchLower)) ||
                    (!string.IsNullOrEmpty(campaign.Description) && campaign.Description.ToLower().Contains(searchLower))
                );
            }

            switch (CurrentFilter)
            {
                case "trending":
                    filteredList = filteredList.OrderByDescending(c => c.Title);
                    break;
                case "nearyou":
                    filteredList = filteredList.OrderBy(c => c.Title);
                    break;
                case "nonprofits":
                    
                    filteredList = filteredList.Where(c => c.Title == "test");
                    break;
                default:
                    filteredList = filteredList.OrderByDescending(c => c.Title);
                    break;
            }

            Campaigns.Clear();
            foreach (var campaign in filteredList)
            {
                Campaigns.Add(campaign);
            }
        }

        public void RefreshCampaigns()
        {
            _ = LoadCampaignsAsync();
        }
    }
}