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
        private ObservableCollection<Campaign> _allCampaigns;
        private ICampaignService _campaignService;
        private IUserSessionService _userSessionService;
        private string _selectedTab = "Verified"; 

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

        public ObservableCollection<Campaign> AllCampaigns
        {
            get => _allCampaigns;
            set
            {
                _allCampaigns = value;
                OnPropertyChanged();
            }
        }

        public string SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                OnPropertyChanged();
                FilterCampaignsByTab();
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
        public RelayCommand SelectVerifiedTab { get; set; }
        public RelayCommand SelectInReviewTab { get; set; }
        public RelayCommand SelectRejectedTab { get; set; }

        public UserCampaignsViewModel(INavigationService navService, ICampaignService campaignService, IUserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
            Navigation = navService;
            _campaignService = campaignService;

            ViewDetails = new RelayCommand(campaign_id => Navigation.NavigateTo<CampaignDetailsViewModel>(campaign_id));

            // Tab selection commands
            SelectVerifiedTab = new RelayCommand(o => SelectedTab = "Verified");
            SelectInReviewTab = new RelayCommand(o => SelectedTab = "In Review");
            SelectRejectedTab = new RelayCommand(o => SelectedTab = "Rejected");

            Campaigns = new ObservableCollection<Campaign>();
            AllCampaigns = new ObservableCollection<Campaign>();
            LoadCampaignsCommand = new RelayCommand(async o => await LoadCampaignsAsync());

            _ = LoadCampaignsAsync();
        }

        private async Task LoadCampaignsAsync()
        {
            try
            {
                Debug.WriteLine($"Current User: {_userSessionService.CurrentUser.Id}");
                var campaigns = await _campaignService.GetAllCampaignsByIdAsync(_userSessionService.CurrentUser.Id, new CampaignIncludeOptions { IncludeDonationRecord = true});

                AllCampaigns.Clear();
                foreach (var campaign in campaigns)
                {
                    AllCampaigns.Add(campaign);
                }

                // Filter campaigns based on selected tab
                FilterCampaignsByTab();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading campaigns: {ex.Message}");
            }
        }

        private void FilterCampaignsByTab()
        {
            if (AllCampaigns == null) return;

            var filteredCampaigns = AllCampaigns.Where(c =>
                string.Equals(c.Status, SelectedTab, StringComparison.OrdinalIgnoreCase)).ToList();

            Campaigns.Clear();
            foreach (var campaign in filteredCampaigns)
            {
                Campaigns.Add(campaign);
            }
        }

        // Properties for tab styling (active/inactive state)
        public bool IsVerifiedTabActive => SelectedTab == "Verified";
        public bool IsInReviewTabActive => SelectedTab == "In Review";
        public bool IsRejectedTabActive => SelectedTab == "Rejected";

        // Override OnPropertyChanged to trigger UI updates for tab states
        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // When SelectedTab changes, notify the tab state properties
            if (propertyName == nameof(SelectedTab))
            {
                OnPropertyChanged(nameof(IsVerifiedTabActive));
                OnPropertyChanged(nameof(IsInReviewTabActive));
                OnPropertyChanged(nameof(IsRejectedTabActive));
            }
        }
    }
}