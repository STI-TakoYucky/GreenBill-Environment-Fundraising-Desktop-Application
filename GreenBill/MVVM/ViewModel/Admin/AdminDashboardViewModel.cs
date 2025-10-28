using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GreenBill.MVVM.ViewModel.Admin {

    public class Campaign {
        public int CampaignID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TargetDonation { get; set; }
        public string AccumulatedDonation { get; set; }
        public string Status { get; set; }
        public bool Verified { get; set; }
        public DateTime DateSubmitted { get; set; }
    }
    public class AdminDashboardViewModel : Core.ViewModel, INavigationAware {

        public ObservableCollection<Campaign> Campaigns { get; set; }

        public SeriesCollection Series { get; set; }
            = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Phase 1",
                    Values = new ChartValues<double> { 2 },
                    DataLabels = true
                },
                new PieSeries {
                    Title = "Phase 2",
                    Values = new ChartValues<double> { 4 },
                    DataLabels = true
                },
                new PieSeries {
                    Title = "Phase 3",
                    Values = new ChartValues<double> { 1 },
                    DataLabels = true
                },
                new PieSeries {
                    Title = "Phase 4",
                    Values = new ChartValues<double> { 4 },
                    DataLabels = true
                },
                new PieSeries {
                    Title = "Phase 5",
                    Values = new ChartValues<double> { 3 },
                    DataLabels = true
                }
            };

        public bool ShowNavigation => true;

        // --- User counting (from earlier change) ---
        private readonly IUserService _userService;
        public List<User> usersFromDB { get; private set; }
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

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
        // --- end user counting ---

        // --- Campaign counts ---
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
        // --- end campaign counts ---

        // Constructor: ICampaignService injected by DI
        public AdminDashboardViewModel(ICampaignService campaignService) {
            _campaignService = campaignService ?? throw new ArgumentNullException(nameof(campaignService));

            Campaigns = new ObservableCollection<Campaign>();

            // sample placeholder (kept for immediate UI)
            Campaigns.Add(new Campaign {
                CampaignID = 1,
                Title = "Charity Run",
                Description = "A charity run for climate change",
                TargetDonation = "1000PHP",
                AccumulatedDonation = "200PHP",
                Status = "Ongoing",
                Verified = true,
                DateSubmitted = DateTime.Now
            });

            Campaigns.Add(new Campaign {
                CampaignID = 2,
                Title = "School Build",
                Description = "Fund a local school",
                TargetDonation = "5000PHP",
                AccumulatedDonation = "1200PHP",
                Status = "Ongoing",
                Verified = true,
                DateSubmitted = DateTime.Now
            });

            // keep same pattern as UserAnalyticsViewModel for user loading
            _userService = new UserService();
            _ = LoadUsersAsync();

            // start loading campaigns and counts
            _ = LoadCampaignCountsAsync();
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

        private async Task LoadCampaignCountsAsync() {
            try {
                var campaignsFromDb = await _campaignService.GetAllCampaignsAsync();

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
                Campaigns.Clear();
                int idx = 1;
                foreach (var c in campaignsFromDb) {
                    Campaigns.Add(new Campaign {
                        CampaignID = idx++,
                        Title = c.Title,
                        Description = c.Description,
                        Status = c.Status,
                        Verified = string.Equals(c.Status, "Verified", StringComparison.OrdinalIgnoreCase),
                        DateSubmitted = c.CreatedAt
                    });
                }
            } catch (Exception ex) {
                MessageBox.Show($"Error loading campaigns: {ex.Message}");
            }
        }
    }
}