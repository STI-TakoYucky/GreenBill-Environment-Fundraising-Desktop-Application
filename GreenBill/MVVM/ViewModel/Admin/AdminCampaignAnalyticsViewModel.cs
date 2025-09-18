using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel.Admin {
    internal class AdminCampaignAnalyticsViewModel : Core.ViewModel, INavigatableService {
        private readonly ICampaignService _campaignService;
        private readonly IUserService _userService;

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

        public AdminCampaignAnalyticsViewModel() {
            _userService = new UserService();
            _campaignService = new CampaignService(_userService);

            _ = LoadCampaignsAsync();
        }

        public void ApplyNavigationParameter(object parameter) {
            if (parameter is ICommand cmd) {
                NavigateToCampaignDetails = cmd;
            }
        }

        private async Task LoadCampaignsAsync() {
            campaignsFromDB = await _campaignService.GetAllCampaignsAsync();
            if (campaignsFromDB == null) return;

            try {
                CampaignCount = campaignsFromDB.Count.ToString();
                Campaigns.Clear();

                foreach (var item in campaignsFromDB) {
                    Campaigns.Add(new MVVM.Model.Campaign {
                        Id = item.Id,
                        UserId = item.UserId,
                        Country = item.Country,
                        ZipCode = item.ZipCode,
                        Category = item.Category,
                        DonationGoal = item.DonationGoal,
                        DonationRaised = item.DonationRaised,
                        Image = item.Image,
                        Title = item.Title,
                        Description = item.Description,
                        CreatedAt = item.CreatedAt,
                        Status = item.Status,
                        User = item.User
                    });
                }

                OnPropertyChanged(nameof(Campaigns));
            } catch (Exception ex) {
                MessageBox.Show($"Error fetching campaigns: {ex.Message}");
            }
        }
    }
}
