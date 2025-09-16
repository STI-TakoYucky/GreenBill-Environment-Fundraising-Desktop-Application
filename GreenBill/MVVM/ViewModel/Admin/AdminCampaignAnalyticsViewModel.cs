using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace GreenBill.MVVM.ViewModel.Admin {
    internal class AdminCampaignAnalyticsViewModel:Core.ViewModel {
        private readonly ICampaignService _campaignService;
        private readonly IUserService _userService;
        private INavigationService _navigationService;
        public INavigationService Navigation {
            get => _navigationService;
            set {
                _navigationService = value;
                OnPropertyChanged();
            }
        }
        public ICommand ReviewCommand { get; }
        private List<GreenBill.MVVM.Model.Campaign> campaignsFromDB { get; set; }
        public ObservableCollection<GreenBill.MVVM.Model.Campaign> Campaigns { get; set; } = new ObservableCollection<GreenBill.MVVM.Model.Campaign>();
        private string _campaignCount;
        public string CampaignCount {
            get => _campaignCount; set {
                if (_campaignCount != value) {
                    _campaignCount = value;
                    OnPropertyChanged(nameof(CampaignCount)); // tells WPF to refresh bindings
                }
                ;
            }
        }

        private GreenBill.MVVM.Model.Campaign _selectedCampaign;
        public GreenBill.MVVM.Model.Campaign SelectedCampaign {
            get => _selectedCampaign;
            set {
                if (_selectedCampaign != value) {
                    _selectedCampaign = value;
                    OnPropertyChanged(nameof(SelectedCampaign));
                }
            }
        }

        public ICommand NavigateToCampaignDetails { get; }

        public AdminCampaignAnalyticsViewModel(INavigationService navService) {
            // Create service manually (not via DI)
            _campaignService = new CampaignService(_userService);
            Navigation = navService;
            NavigateToCampaignDetails = new RelayCommand(campaign_id => Navigation.NavigateTo<ReviewCampaignViewModel>(campaign_id.ToString()));
            _ = LoadCampaignsAsync();

        }




        private async Task LoadCampaignsAsync() {
            campaignsFromDB = await _campaignService.GetAllCampaignsAsync();
            if (campaignsFromDB == null) return;

            try {
                CampaignCount = campaignsFromDB.Count.ToString();
                foreach (var item in campaignsFromDB) {
                    Campaigns.Add(new GreenBill.MVVM.Model.Campaign {
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
                OnPropertyChanged(nameof(campaignsFromDB));
            } catch (Exception ex) {
                MessageBox.Show($"Error fetching users: {ex.Message}");
            }
        }
    }
}
