using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using LiveCharts;
using LiveCharts.Wpf;
using LiveChartsCore;
using Stripe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GreenBill.MVVM.ViewModel.Admin {
    internal class AdminCampaignAnalyticsViewModel : Core.ViewModel, INavigatableService {
        private readonly ICampaignService _campaignService;
        private readonly IUserService _userService;
        private string _selectedTab = "All";
        public SeriesCollection CartesianSeries { get; set; }
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

        private string _rejectedCampaignsCount;

        public string RejectedCampaignsCount {
            get => _rejectedCampaignsCount;
            set {
                _rejectedCampaignsCount = value;
                OnPropertyChanged(nameof(RejectedCampaignsCount));
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
        public Func<double, string> DateFormatter { get; set; }

        public AdminCampaignAnalyticsViewModel(ICampaignService campaignService, IUserService userService, ITabNavigationService navService) {
            _campaignService = campaignService;
            _userService = userService;
            _navigationService = navService;
            _ = LoadCampaignsAsync();

            SelectAllCampaigns = new RelayCommand(o => SelectedTab = "All");
            SelectVerifiedCampaigns = new RelayCommand(o => SelectedTab = "Verified");
            SelectPendingCampaigns = new RelayCommand(o => SelectedTab = "in review");
            SelectRejectedCampaigns = new RelayCommand(o => SelectedTab = "rejected");
            NavigateToCampaignDetails = new RelayCommand(campaign_id => { Navigation?.NavigateToTab<ReviewCampaignViewModel>(campaign_id.ToString()); });
        }

        // Called by navigation service (reflection) and view code to refresh data
        public void Refresh() {
            _ = LoadCampaignsAsync();
        }

        // Support navigation parameter injection (keeps backward compatibility)
        public void ApplyNavigationParameter(object parameter) {
            if (parameter is ICommand cmd) {
                // only set if caller provided a navigation command
                NavigateToCampaignDetails = cmd;
            }
            // always refresh when navigated to
            Refresh();
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

        private void BuildLineChart(List<Campaign> campaignsAsync) {
            List<DateTime> ChartDates = new List<DateTime>();
            var campaigns = campaignsAsync;

            var campaignValues = new ChartValues<int>();
            var userValues = new ChartValues<int>();
            ChartDates.Clear();

            if (campaigns == null || campaigns.Count == 0)
                return;

            DateTime campaignCreatedDate = campaigns.OrderBy(c => c.CreatedAt).FirstOrDefault()?.CreatedAt.Date ?? DateTime.Now.Date;

            DateTime startDate = campaignCreatedDate;
            DateTime endDate = DateTime.Now.Date;

            // Build a list of all dates to plot
            var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                     .Select(offset => startDate.AddDays(offset))
                                     .ToList();

            foreach (var day in allDates) {
                int campaignCountPerDay = campaigns.Count(c => c.CreatedAt.Date == day);

                // Only skip days where both counts are zero, except start/end
                if ((campaignCountPerDay == 0) && day != startDate && day != endDate)
                    continue;

                ChartDates.Add(day);
                campaignValues.Add(campaignCountPerDay);
            }

            DateFormatter = value => {
                int index = (int)value;
                if (index < 0 || index >= ChartDates.Count) return "";
                return ChartDates[index].ToString("MMM dd yyyy");
            };
            OnPropertyChanged(nameof(DateFormatter));

                CartesianSeries = new SeriesCollection
                       {
                    new LineSeries
                    {
                        Title = "Campaign Submissions",
                        Values = campaignValues,
                        PointGeometry = DefaultGeometries.Circle,
                        StrokeThickness = 2,
                        LineSmoothness= 0,
                        DataLabels = false,
                        Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#00A86B")),
                        Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#2000A86B")),
                        LabelPoint = chartPoint =>
                            $"{ChartDates[(int)chartPoint.Key]:MMM dd, yyyy} → {chartPoint.Y}"
                    }
                };
            

            OnPropertyChanged(nameof(CartesianSeries));
        }

        private async Task LoadCampaignsAsync() {
            campaignsFromDB = await _campaignService.GetAllCampaignsAsync();
            if (campaignsFromDB == null) return;

            try {
                CampaignCount = campaignsFromDB.Count.ToString();
                ApprovedCampaignsCount = campaignsFromDB.Count(c => c.Status.ToLower() == "verified").ToString();
                PendingCampaignsCount = campaignsFromDB.Count(c => c.Status.ToLower() == "in review").ToString();
                RejectedCampaignsCount = campaignsFromDB.Count(c => c.Status.ToLower() == "rejected").ToString();
                Campaigns.Clear();

                foreach (var item in campaignsFromDB) {
                    Campaigns.Add(item);
                }

                OnPropertyChanged(nameof(Campaigns));
                FilterCampaignsByTab();
                BuildLineChart(campaignsFromDB);
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
