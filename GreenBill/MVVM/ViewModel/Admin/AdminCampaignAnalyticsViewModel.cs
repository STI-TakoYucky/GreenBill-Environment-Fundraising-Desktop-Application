using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel.Admin {
    internal class AdminCampaignAnalyticsViewModel : Core.ViewModel, INavigatableService {
        private readonly ICampaignService _campaignService;
        private readonly IUserService _userService;
        private string _selectedTab = "All";

        public string SelectedTab {
            get => _selectedTab;
            set {
                _selectedTab = value;
                OnPropertyChanged();
                FilterCampaignsByTab();
            }
        }

        private ITabNavigationService _navigationService;
        public ITabNavigationService Navigation {
            get => _navigationService;
            set {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        private List<MVVM.Model.Campaign> campaignsFromDB { get; set; }

        public ObservableCollection<MVVM.Model.Campaign> Campaigns { get; set; } = new ObservableCollection<MVVM.Model.Campaign>();

        private string _campaignCount;
        public string CampaignCount {
            get => _campaignCount;
            set {
                if (_campaignCount != value) {
                    _campaignCount = value;
                    OnPropertyChanged(nameof(CampaignCount));
                }
            }
        }

        private string _approvedCampaignsCount;

        public string ApprovedCampaignsCount { 
            get => _approvedCampaignsCount; 
            set { 
                if (value != _approvedCampaignsCount) {
                    _approvedCampaignsCount = value;
                    OnPropertyChanged(nameof(ApprovedCampaignsCount));
                }        
            } 
        }

        private string _pendingCampaignsCount;

        public string PendingCampaignsCount {
            get => _pendingCampaignsCount;
            set {
                if (value != _pendingCampaignsCount) {
                    _pendingCampaignsCount = value;
                    OnPropertyChanged(nameof(PendingCampaignsCount));
                }
            }
        }

        private Campaign _selectedCampaign;
        public Campaign SelectedCampaign {
            get => _selectedCampaign;
            set {
                if (_selectedCampaign != value) {
                    _selectedCampaign = value;
                    OnPropertyChanged(nameof(SelectedCampaign));
                }
            }
        }

        // Will be injected via ApplyNavigationParameter
        public ICommand NavigateToCampaignDetails { get; private set; }
        public RelayCommand SelectAllCampaigns { get; set; }
        public RelayCommand SelectVerifiedCampaigns { get; set; }
        public RelayCommand SelectPendingCampaigns { get; set; }
        public RelayCommand SelectRejectedCampaigns { get; set; }

        public AdminCampaignAnalyticsViewModel(ICampaignService campaignService, IUserService userService) {
            _campaignService = campaignService;
            _userService = userService;
            _ = LoadCampaignsAsync();

            SelectAllCampaigns = new RelayCommand(o => SelectedTab = "All");
            SelectVerifiedCampaigns = new RelayCommand(o => SelectedTab = "Verified");
            SelectPendingCampaigns = new RelayCommand(o => SelectedTab = "in review");
            SelectRejectedCampaigns = new RelayCommand(o => SelectedTab = "rejected");
        }

        private void FilterCampaignsByTab() {
            if (campaignsFromDB == null) return;
            Campaigns.Clear();

            if (SelectedTab == "All") {
                foreach (var campaign in campaignsFromDB)
                    Campaigns.Add(campaign);
                return;
            }

            var filteredCampaigns = campaignsFromDB.Where(c =>
                string.Equals(c.Status, SelectedTab, StringComparison.OrdinalIgnoreCase)).ToList();

            Campaigns.Clear();
            foreach (var campaign in filteredCampaigns) {
                Campaigns.Add(campaign);
            }
        }

        public void ApplyNavigationParameter(object parameter) {
            if (parameter is ICommand cmd) {
                NavigateToCampaignDetails = cmd;
            }
            _ = LoadCampaignsAsync();
        }

        private async Task LoadCampaignsAsync() {
            campaignsFromDB = await _campaignService.GetAllCampaignsAsync();
            if (campaignsFromDB == null) return;

            try {
                CampaignCount = campaignsFromDB.Count.ToString();
                ApprovedCampaignsCount = campaignsFromDB.Count(c => c.Status == "Verified").ToString();
                PendingCampaignsCount = campaignsFromDB.Count(c => c.Status == "in review").ToString();
                Campaigns.Clear();

                foreach (var item in campaignsFromDB) {
                    Campaigns.Add(item);
                }

                OnPropertyChanged(nameof(Campaigns));
                FilterCampaignsByTab();
            } catch (Exception ex) {
                MessageBox.Show($"Error fetching campaigns: {ex.Message}");
            }
        }

        // Properties for tab styling (active/inactive state)
        public bool IsAllCampaignsActive => SelectedTab == "All";
        public bool IsVerifiedCampaignsActive => SelectedTab == "Verified";
        public bool IsPendingCampaignsActive => SelectedTab == "in review";
        public bool IsRejectedCampaignsActive => SelectedTab == "rejected";

        // Override OnPropertyChanged to trigger UI updates for tab states
        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) {
            base.OnPropertyChanged(propertyName);

            // When SelectedTab changes, notify the tab state properties
            if (propertyName == nameof(SelectedTab)) {
                OnPropertyChanged(nameof(IsAllCampaignsActive));
                OnPropertyChanged(nameof(IsVerifiedCampaignsActive));
                OnPropertyChanged(nameof(IsPendingCampaignsActive));
                OnPropertyChanged(nameof(IsRejectedCampaignsActive));
            }
        }

    }
}
