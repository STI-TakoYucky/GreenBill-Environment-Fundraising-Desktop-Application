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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace GreenBill.MVVM.ViewModel.Admin {

    public class AdminDashboardViewModel : Core.ViewModel, INavigationAware, INotifyPropertyChanged, INavigatableService {

        public SeriesCollection CartesianSeries { get; set; }
        public List<User> usersFromDB { get; private set; }
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<MVVM.Model.Campaign> Campaigns { get; set; } = new ObservableCollection<MVVM.Model.Campaign>();
        public bool ShowNavigation => true;

        //GETTERS AND SETTERS START HERE

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

        private DateTime _date;
        private DateTime Date {
            get => _date;
            set {
                _date = value;
            }
        }
        //GETTERS AND SETTERS END HERE

        //COMMANDS
        public ICommand NavigateToCampaignDetails { get; set; }
        public ICommand NavigateToCampaignAnalytics { get; set; }
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
        }

        private async Task LoadUsersAsync() {
            try {
                usersFromDB = await _userService.GetAllUsersAsync();
                if (usersFromDB == null) return;

                UserCount = usersFromDB.Count.ToString();

                Users.Clear();
                foreach (var item in usersFromDB) {
                    Users.Add(new User {
                        Id = item.Id,
                        Profile = item.Profile,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Username = item.Username,
                        Email = item.Email,
                        Password = item.Password,
                        CreatedAt = item.CreatedAt
                    });
                }

                OnPropertyChanged(nameof(usersFromDB));
            } catch (Exception ex) {
                MessageBox.Show($"Error fetching users: {ex.Message}");
            }
        }

        private async Task LoadCampaignAsync() {
            try {
                var campaignsFromDb = await _campaignService.GetAllCampaignsAsync();
                    campaignsFromDb = campaignsFromDb?.OrderByDescending(c => c.CreatedAt).ToList();
                if (campaignsFromDb == null) {
                    TotalCampaigns = "0";
                    ActiveCampaigns = "0";
                    PendingCampaigns = "0";
                    return;
                }

                // counts
                TotalCampaigns = campaignsFromDb.Count.ToString();
                PendingCampaigns = campaignsFromDb.Count(c => string.Equals(c.Status, "in review", StringComparison.OrdinalIgnoreCase)).ToString();
                ActiveCampaigns = campaignsFromDb.Count(c => string.Equals(c.Status, "Verified", StringComparison.OrdinalIgnoreCase)).ToString();

                // populate the Campaigns collection (map minimal fields)
                Campaigns.Clear(); //reset the campaigns collection
                string selected = SelectedPeriod.Content.ToString();

                DateTime startDate = selected == "All Time"
               ? campaignsFromDb.OrderBy(c => c.CreatedAt).First().CreatedAt.Date // get the earliest date from the campaigns
               : Date.Date;  // filtered starting date from the combo box

                foreach (var c in campaignsFromDb) {
                    if (!string.Equals(c.Status, "in review", StringComparison.OrdinalIgnoreCase))
                        continue;

                    //if selected all time, just populate. //startDate is the earliest date based on the campaigns
                    if (selected == "All Time" || (c.CreatedAt >= startDate && c.CreatedAt <= DateTime.Now)) {
                        Campaigns.Add(c);
                    }
                }

                OnPropertyChanged(nameof(Campaigns));
                BuildSeriesFromCampaignDates(campaignsFromDb.ToList(), selected);
            } catch (Exception ex) {
                MessageBox.Show($"Error loading campaigns: {ex.Message}");
            }
        }
        public List<DateTime> ChartDates { get; set; } = new List<DateTime>();

        private void BuildSeriesFromCampaignDates(List<MVVM.Model.Campaign> campaigns, string period) {
            var values = new ChartValues<double>();
            ChartDates.Clear();
            values.Clear();


            // Determine start date based on filter
            DateTime startDate = period == "All Time"
                ? campaigns.OrderBy(c => c.CreatedAt).First().CreatedAt.Date // get the earliest date from the campaigns
                : Date.Date;  // filtered starting date from the combo box

            DateTime endDate = DateTime.Now.Date;

            // Loop through every day between start and end
            for (DateTime day = startDate; day <= endDate; day = day.AddDays(1)) {
                int countForTheDay = campaigns.Count(c =>
                    c.CreatedAt.Date == day.Date
                );

                if(day == startDate || day == endDate) { // to plot where the first and the last date is even if it is zero
                    ChartDates.Add(day);   // store the date
                    values.Add(countForTheDay);
                    continue;
                }

                if (countForTheDay <= 0) {
                    continue;
                }

                ChartDates.Add(day);   // store the date
                values.Add(countForTheDay);
            }

            DateFormatter = value =>
            {
                int index = (int)value;
                if (index < 0 || index >= ChartDates.Count) return "";
                return ChartDates[index].ToString("MMM dd yyyy");
            };

            OnPropertyChanged(nameof(DateFormatter));


            // Build the line chart series
            CartesianSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Campaign Submissions",
                    Values = values,
                    PointGeometry = DefaultGeometries.Circle,
                    StrokeThickness = 2,
                    DataLabels = false,
                    LabelPoint = chartPoint =>
                        $"{ChartDates[(int)chartPoint.Key]:MMM dd, yyyy} → {chartPoint.Y}"
                }
            };


            OnPropertyChanged(nameof(CartesianSeries));
        }




        private void ApplyDateFilter(string period) {
            DateTime currDate = DateTime.Now;
            if (string.IsNullOrEmpty(period)) {
                return;
            }

            switch (period) {
                case "All Time":
                    Date = DateTime.Now;
                    break;

                case "Last 7 days":
                    Date = currDate.AddDays(-7);
                    break;

                case "Last 30 days":
                    Date = currDate.AddDays(-30);
                    break;

                case "Last 3 months":
                    Date = currDate.AddMonths(-3);
                    break;

                case "Last 6 months":
                    Date = currDate.AddMonths(-6);
                    break;

                case "Last year":
                    Date = currDate.AddYears(-1);
                    break;
            }

            _ = LoadCampaignAsync();
        }

        public void ApplyNavigationParameter(object parameter) {
            // we don't use parameter here, just refresh when navigated to
            // start loading campaigns and counts, and users
            _ = LoadUsersAsync();
            _ = LoadCampaignAsync();
            _selectedPeriod = new ComboBoxItem { Content = "All Time" };
        }

    }
}