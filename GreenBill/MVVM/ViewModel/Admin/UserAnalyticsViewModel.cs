using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using LiveCharts;
using LiveCharts.Wpf;
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
    internal class UserAnalyticsViewModel : Core.ViewModel {
        //set variables
        private readonly IUserService _userService;
        public List<User> usersFromDB { get; private set; }
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

        public SeriesCollection CartesianSeries { get; set; }

        //usercount
        private string _userCount;
        public string UserCount {
            get => _userCount;
            set {
                if (_userCount != value) {
                    _userCount = value;
                    OnPropertyChanged(nameof(UserCount)); // tells WPF to refresh bindings
                }
            }
        }

        //verified user count

        private string _verifiedUserCount;
        public string VerifiedUserCount {
            get => _verifiedUserCount;
            set {
                _verifiedUserCount = value;
                OnPropertyChanged(nameof(VerifiedUserCount));
            }
        }

        //pending user count, this is for stripe
        private string _pendingUserCount;
        public string PendingUserCount {
            get => _pendingUserCount;
            set {
                _pendingUserCount = value;
                OnPropertyChanged(nameof(PendingUserCount));
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
        public ICommand NavigateToReviewUser { get; set; }
        public ICommand TabSelect { get; set; }
        private string _activeTab = "all";

        public string activeTab {
            get => _activeTab;
            set {
                _activeTab = value;
                OnPropertyChanged();
                FilterTab();
            }
        }

        public Func<double, string> DateFormatter { get; set; }

        // Default constructor used by TabNavigationService
        public UserAnalyticsViewModel(ITabNavigationService navigationService) {
            // Create service manually (not via DI)
            _userService = new UserService();
            Navigation = navigationService;
            _ = LoadUsersAsync();

            NavigateToReviewUser = new RelayCommand(userId => Navigation.NavigateToTab<ReviewUserViewModel>(userId.ToString()));
            TabSelect = new RelayCommand(tab => activeTab = tab.ToString().ToLower());
        }

        private void FilterTab() {
            Users.Clear();
            if (activeTab.ToLower() == "all") {
                foreach (var item in usersFromDB) {
                    if (item.Role == "user") {
                        Users.Add(item);
                    }
                }
                return;
            }

            var filteredUsers = usersFromDB.Where(c => string.Equals(c.VerificationStatus.ToLower(), activeTab, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var item in filteredUsers) {
                if(item.Role == "user") {
                    Users.Add(item);
                }
            }
        }

        private async Task LoadUsersAsync() {
            usersFromDB = await _userService.GetAllUsersAsync();
            if (usersFromDB == null) return;

            try {
                UserCount = usersFromDB.Count(user => user.Role == "user").ToString();
                VerifiedUserCount = usersFromDB.Count(item => item.VerificationStatus.ToLower() == "verified" && item.Role == "user").ToString();
                PendingUserCount = usersFromDB.Count(item => item.VerificationStatus.ToLower() == "pending" && item.Role == "user").ToString();

                OnPropertyChanged(nameof(usersFromDB));
                FilterTab();
                BuildLineChart(usersFromDB);
            } catch (Exception ex) {
                MessageBox.Show($"Error fetching users: {ex.Message}");
            }
        }
        private void BuildLineChart(List<User> usersFromDB) {
            List<DateTime> ChartDates = new List<DateTime>();
            var users = usersFromDB;

            var userValues = new ChartValues<int>();
            ChartDates.Clear();

            if (users == null || users.Count == 0) return;

            DateTime userCreatedDate = users.OrderBy(u => u.CreatedAt).FirstOrDefault()?.CreatedAt.Date ?? DateTime.Now.Date;


            DateTime startDate = userCreatedDate;
            DateTime endDate = DateTime.Now.Date;

            // Build a list of all dates to plot
            var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                     .Select(offset => startDate.AddDays(offset))
                                     .ToList();

            foreach (var day in allDates) {
                int userCountPerDay = users.Count(u => u.CreatedAt.Date == day);

                // Only skip days where counts are zero, except start/end
                if ((userCountPerDay == 0) && day != startDate && day != endDate)
                    continue;

                ChartDates.Add(day);
                userValues.Add(userCountPerDay);
            }

            DateFormatter = value => {
                int index = (int)value;
                if (index < 0 || index >= ChartDates.Count) return "";
                return ChartDates[index].ToString("MMM dd yyyy");
            };

            OnPropertyChanged(nameof(DateFormatter));
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

            OnPropertyChanged(nameof(CartesianSeries));
        }


        // Properties for tab styling (active/inactive state)
        public bool isAllUsersActive => activeTab == "all";
        public bool isVerifiedUsersActive => activeTab == "verified";
        public bool isPendingUsersActive => activeTab == "pending";

        protected override void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) {
            base.OnPropertyChanged(propertyName);

            // When SelectedTab changes, notify the tab state properties
            if (propertyName == nameof(activeTab)) {
                OnPropertyChanged(nameof(isAllUsersActive));
                OnPropertyChanged(nameof(isVerifiedUsersActive));
                OnPropertyChanged(nameof(isPendingUsersActive));
            }
        }
    }
}
