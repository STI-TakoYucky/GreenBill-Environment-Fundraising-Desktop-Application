using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.MVVM.View;
using GreenBill.Services;
using LiveCharts;
using LiveCharts.Wpf;
using Stripe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace GreenBill.MVVM.ViewModel.Admin {

    public class AdminDashboardViewModel : Core.ViewModel, INavigationAware, INotifyPropertyChanged, INavigatableService {

        public SeriesCollection CartesianSeries { get; set; }
        public SeriesCollection PieSeries { get; set; } = new SeriesCollection();
        public ObservableCollection<MVVM.Model.User> Users { get; set; } = new ObservableCollection<MVVM.Model.User>();
        public ObservableCollection<MVVM.Model.Campaign> Campaigns { get; set; } = new ObservableCollection<MVVM.Model.Campaign>();
        public bool ShowNavigation => true;

        //GETTERS AND SETTERS START HERE

        private string _selectedTab = "Campaigns";

        public string SelectedTab {
            get => _selectedTab;
            set {
                _selectedTab = value;
                OnPropertyChanged();
                BuildLineChart(LoadUsersAsync(), LoadCampaignAsync());
                BuildPieChart();
            }
        }

        private readonly IUserService _userService;
        private ITabNavigationService _navigationService;
        public ITabNavigationService Navigation {
            get => _navigationService;
            set {
                _navigationService = value;
                OnPropertyChanged();
            }
        }
        private string _userCount;
        public string UserCount {
            get => _userCount;
            set {
                if (_userCount != value) {
                    _userCount = value;
                    OnPropertyChanged(nameof(UserCount));
                }
            }
        }
        private readonly ICampaignService _campaignService;

        private string _totalCampaigns;
        public string TotalCampaigns {
            get => _totalCampaigns;
            set {
                if (_totalCampaigns != value) {
                    _totalCampaigns = value;
                    OnPropertyChanged(nameof(TotalCampaigns));
                }
            }
        }

        private string _activeCampaigns;
        public string ActiveCampaigns {
            get => _activeCampaigns;
            set {
                if (_activeCampaigns != value) {
                    _activeCampaigns = value;
                    OnPropertyChanged(nameof(ActiveCampaigns));
                }
            }
        }

        private string _pendingCampaigns;
        public string PendingCampaigns {
            get => _pendingCampaigns;
            set {
                if (_pendingCampaigns != value) {
                    _pendingCampaigns = value;
                    OnPropertyChanged(nameof(PendingCampaigns));
                }
            }
        }

        private ComboBoxItem _selectedPeriod;
        public ComboBoxItem SelectedPeriod {
            get => _selectedPeriod;
            set {
                if (_selectedPeriod == value) return;
                _selectedPeriod = value;
                OnPropertyChanged(nameof(SelectedPeriod));
                ApplyDateFilter(_selectedPeriod.Content.ToString());
            }
        }

        private DateTime _comboBoxDate;
        private DateTime ComboBoxDate {
            get => _comboBoxDate;
            set {
                _comboBoxDate = value;
            }
        }
        //GETTERS AND SETTERS END HERE

        //COMMANDS
        public ICommand NavigateToCampaignDetails { get; set; }
        public ICommand NavigateToCampaignAnalytics { get; set; }
        public ICommand SelectCampaignsTab { get; set; }
        public ICommand SelectUsersTab { get; set; }
        public Func<double, string> DateFormatter { get; set; }
        //COMMANDS

        public AdminDashboardViewModel(ICampaignService campaignService, ITabNavigationService navService, IUserService userService) {
            _campaignService = campaignService;
            _navigationService = navService;
            _userService = userService;
            _selectedPeriod = new ComboBoxItem { Content = "All Time" };

            //initialize commands
            NavigateToCampaignDetails = new RelayCommand(campaign_id => { Navigation?.NavigateToTab<ReviewCampaignViewModel>(campaign_id.ToString()); });
            NavigateToCampaignAnalytics = new RelayCommand(o => Navigation.NavigateToTab<AdminCampaignAnalyticsViewModel>());
            SelectCampaignsTab = new RelayCommand(_ => SelectedTab = "Campaigns");
            SelectUsersTab = new RelayCommand(_ => SelectedTab = "Users");
        }

        private async Task<List<User>> LoadUsersAsync() {
            try {
                Users.Clear();
                var usersFromDB = await _userService.GetAllUsersAsync();
                if (usersFromDB == null) return null;
                UserCount = usersFromDB.Count.ToString();
                string selected = SelectedPeriod.Content.ToString();// selected period

                DateTime startDate = selected == "All Time"
                  ? usersFromDB.OrderBy(c => c.CreatedAt).First().CreatedAt.Date // get the earliest date from the users
                  : ComboBoxDate.Date;  // filtered starting date from the combo box

                foreach (var item in usersFromDB) {
                    //if selected all time, just populate. //startDate is the earliest date based on the users
                    if (selected == "All Time" || (item.CreatedAt >= startDate && item.CreatedAt <= DateTime.Now)) {
                        Users.Add(item);
                    }
                }

                OnPropertyChanged(nameof(usersFromDB));
                return usersFromDB;
            } catch (Exception ex) {
                MessageBox.Show($"Error fetching users: {ex.Message}");
                return null;
            }
        }

        private async Task<List<Campaign>> LoadCampaignAsync() {
            try {
                Campaigns.Clear(); //reset the campaigns collection
                var campaignsFromDb = await _campaignService.GetAllCampaignsAsync();
                campaignsFromDb = campaignsFromDb?.OrderByDescending(c => c.CreatedAt).ToList();

                if (campaignsFromDb == null) {
                    TotalCampaigns = "0";
                    ActiveCampaigns = "0";
                    PendingCampaigns = "0";
                    return null;
                }

                // counts
                TotalCampaigns = campaignsFromDb.Count.ToString();
                PendingCampaigns = campaignsFromDb.Count(c => string.Equals(c.Status, "in review", StringComparison.OrdinalIgnoreCase)).ToString();
                ActiveCampaigns = campaignsFromDb.Count(c => string.Equals(c.Status, "Verified", StringComparison.OrdinalIgnoreCase)).ToString();

                // populate the Campaigns collection (map minimal fields)
                string selected = SelectedPeriod.Content.ToString();

                DateTime startDate = selected == "All Time"
               ? campaignsFromDb.OrderBy(c => c.CreatedAt).First().CreatedAt.Date // get the earliest date from the campaigns
               : ComboBoxDate.Date;  // filtered starting date from the combo box

                foreach (var c in campaignsFromDb) {
                    if (!string.Equals(c.Status, "in review", StringComparison.OrdinalIgnoreCase))
                        continue;

                    //if selected all time, just populate. //startDate is the earliest date based on the campaigns
                    if (selected == "All Time" || (c.CreatedAt >= startDate && c.CreatedAt <= DateTime.Now)) {
                        Campaigns.Add(c);
                    }
                }

                OnPropertyChanged(nameof(Campaigns));
                return campaignsFromDb;
            } catch (Exception ex) {
                MessageBox.Show($"Error loading campaigns: {ex.Message}");
                return null;
            }
        }

        private async void BuildLineChart(Task<List<User>> usersAsync, Task<List<Campaign>> campaignsAsync) {
        List<DateTime> ChartDates = new List<DateTime>();
        var users = await usersAsync;
            var campaigns = await campaignsAsync;

            var campaignValues = new ChartValues<int>();
            var userValues = new ChartValues<int>();
            ChartDates.Clear();

            if ((users == null || users.Count == 0) && (campaigns == null || campaigns.Count == 0))
                return;

            DateTime userCreatedDate = users.OrderBy(u => u.CreatedAt).FirstOrDefault()?.CreatedAt.Date ?? DateTime.Now.Date;
            DateTime campaignCreatedDate = campaigns.OrderBy(c => c.CreatedAt).FirstOrDefault()?.CreatedAt.Date ?? DateTime.Now.Date;
            DateTime earliestDate = userCreatedDate < campaignCreatedDate ? userCreatedDate : campaignCreatedDate;

            DateTime startDate = SelectedPeriod.Content.ToString() == "All Time" ? earliestDate : ComboBoxDate.Date;
            DateTime endDate = DateTime.Now.Date;

            // Build a list of all dates to plot
            var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                     .Select(offset => startDate.AddDays(offset))
                                     .ToList();

            foreach (var day in allDates) {
                int userCountPerDay = users.Count(u => u.CreatedAt.Date == day);
                int campaignCountPerDay = campaigns.Count(c => c.CreatedAt.Date == day);

                // Only skip days where both counts are zero, except start/end
                if ((userCountPerDay == 0 && campaignCountPerDay == 0) && day != startDate && day != endDate)
                    continue;

                ChartDates.Add(day);
                campaignValues.Add(campaignCountPerDay);
                userValues.Add(userCountPerDay);
            }

            DateFormatter = value => {
                int index = (int)value;
                if (index < 0 || index >= ChartDates.Count) return "";
                return ChartDates[index].ToString("MMM dd yyyy");
            };
            OnPropertyChanged(nameof(DateFormatter));

           if (SelectedTab == "Campaigns") {
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
            } else if(SelectedTab == "Users") {
                CartesianSeries = new SeriesCollection {
                new LineSeries {
                    Title = "Users",
                    Values = userValues,
                    PointGeometry = DefaultGeometries.Circle,
                    LineSmoothness = 0,
                    StrokeThickness = 2,
                    Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#3498DB")),
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#203498DB")),
                    DataLabels = false,
                    LabelPoint = chartPoint =>
                        $"{ChartDates[(int)chartPoint.Key]:MMM dd, yyyy} → {chartPoint.Y}"
                }
                };
            }

                OnPropertyChanged(nameof(CartesianSeries));
        }

        private readonly List<string> _palette = new List<string>()
            {
                "#00695C", // Deep teal (your base)
                "#FF6F3C", // Bright complimentary orange
                "#00A896", // Vibrant turquoise
                "#FFD166", // Warm golden highlight
                "#EF476F", // Strong contrasting magenta-red
                "#073B4C"  // Deep dark cyan for depth
            };

        private async void BuildPieChart() {
            PieSeries.Clear();

            int colorIndex = 0;

            // --- LOAD DATA BASED ON TAB ---
            if (SelectedTab == "Campaigns") {
                var campaigns = await _campaignService.GetAllCampaignsAsync();

                var grouped = campaigns
                    .GroupBy(c => c.Category)
                    .Select(g => new {
                        Title = g.Key,
                        Count = g.Count()
                    });

                foreach (var item in grouped) {
                    string hex = _palette[colorIndex % _palette.Count];
                    colorIndex++;

                    PieSeries.Add(new PieSeries {
                        Title = item.Title,
                        Values = new ChartValues<int> { item.Count },
                        DataLabels = true,
                        Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(hex)
                    });
                }
            } else if (SelectedTab == "Users") {
                var users = await _userService.GetAllUsersAsync();

                var grouped = users
                    .GroupBy(u => u.VerificationStatus)
                    .Select(g => new {
                        Title = g.Key,
                        Count = g.Count()
                    });

                foreach (var item in grouped) {
                    string hex = _palette[colorIndex % _palette.Count];
                    colorIndex++;

                    PieSeries.Add(new PieSeries {
                        Title = item.Title,
                        Values = new ChartValues<int> { item.Count },
                        DataLabels = true,
                        Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(hex)
                    });
                }
            }
        }





        private void ApplyDateFilter(string period) {
            DateTime currDate = DateTime.Now;
            if (string.IsNullOrEmpty(period)) {
                return;
            }

            switch (period) {
                case "All Time":
                    ComboBoxDate = DateTime.Now;
                    break;

                case "Last 7 days":
                    ComboBoxDate = currDate.AddDays(-7);
                    break;

                case "Last 30 days":
                    ComboBoxDate = currDate.AddDays(-30);
                    break;

                case "Last 3 months":
                    ComboBoxDate = currDate.AddMonths(-3);
                    break;

                case "Last 6 months":
                    ComboBoxDate = currDate.AddMonths(-6);
                    break;

                case "Last year":
                    ComboBoxDate = currDate.AddYears(-1);
                    break;
            }

            BuildLineChart(LoadUsersAsync(), LoadCampaignAsync());
        }

        public void ApplyNavigationParameter(object parameter) {
            // we don't use parameter here, just refresh when navigated to
            // start loading campaigns and counts, and users
            BuildLineChart(LoadUsersAsync(), LoadCampaignAsync());
            _selectedPeriod = new ComboBoxItem { Content = "All Time" };
            BuildPieChart();
        }

        // Properties for tab styling (active/inactive state)
        public bool isCampaignsTabActive => SelectedTab == "Campaigns";
        public bool isUsersTabActive => SelectedTab == "Users";

        // Override OnPropertyChanged to trigger UI updates for tab states
        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) {
            base.OnPropertyChanged(propertyName);

            // When SelectedTab changes, notify the tab state properties
            if (propertyName == nameof(SelectedTab)) {
                OnPropertyChanged(nameof(isCampaignsTabActive));
                OnPropertyChanged(nameof(isUsersTabActive));
            }

        }
    }
}