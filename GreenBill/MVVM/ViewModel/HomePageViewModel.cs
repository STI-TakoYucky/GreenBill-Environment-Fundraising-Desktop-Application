using GreenBill.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GreenBill.Core;
using System.Diagnostics;
using GreenBill.MVVM.Model;
using System.Collections.ObjectModel;
using GreenBill.IServices;
using System.Collections.Generic;
using System.Linq;

namespace GreenBill.MVVM.ViewModel
{
    public class HomePageViewModel : Core.ViewModel, INavigationAware, INavigatableService
    {

        // Projects Funded
        private int _projectsFunded;
        public int ProjectsFunded
        {
            get => _projectsFunded;
            set
            {
                _projectsFunded = value;
                OnPropertyChanged();
            }
        }
        // Raised
        private decimal _raised;
        public decimal Raised
        {
            get => _raised;
            set
            {
                _raised = value;
                OnPropertyChanged();
            }
        }
        // Donors
        private int _donors;
        public int Donors
        {
            get => _donors;
            set
            {
                _donors = value;
                OnPropertyChanged();
            }
        }
        // Campaigns
        private int _campaignCount;
        public int CampaignCount
        {
            get => _campaignCount;
            set
            {
                _campaignCount = value;
                OnPropertyChanged();
            }
        }
        public bool ShowNavigation => true;
        private INavigationService _navigationService;
        private ObservableCollection<Campaign> _campaigns;
        private ICampaignService _campaignService;
        private IDonationRecordService _donationRecordService;
        private IUserSessionService _userSessionService;
        public string _successMessage;
        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                _successMessage = value;
                OnPropertyChanged();
            }
        }

        public bool _showMessage;
        public bool ShowMessage
        {
            get => _showMessage;
            set
            {
                _showMessage = value;
                OnPropertyChanged();
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

        public ObservableCollection<Campaign> Campaigns
        {
            get => _campaigns;
            set
            {
                _campaigns = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateToFundraisingDetails { get; set; }
        public ICommand LoadCampaignsCommand { get; set; }
        public ICommand ViewMore { get; set; }
        public ICommand StartAChange { get; set; }

        public HomePageViewModel(INavigationService navService, ICampaignService campaignService, IDonationRecordService donationRecordService, IUserSessionService userSessionService)
        {
            Navigation = navService;
            _campaignService = campaignService;
            Campaigns = new ObservableCollection<Campaign>();
            _donationRecordService = donationRecordService;
            _userSessionService = userSessionService;

            NavigateToFundraisingDetails = new RelayCommand(campaign_id =>
            {
                if (campaign_id != null)
                {
                    Navigation.NavigateTo<FundraisingDetailsViewModel>(campaign_id.ToString());
                }
            });
            ViewMore = new RelayCommand(o => Navigation.NavigateTo<CampaignsViewModel>());
            LoadCampaignsCommand = new RelayCommand(async o => await LoadCampaignsAsync());
            StartAChange = new RelayCommand(o =>
            {
                if (_userSessionService.IsUserLoggedIn)
                {
                    Navigation.NavigateTo<FundraisingStepsViewModel>();
                }
                else
                {
                    Navigation.NavigateTo<SigninViewModel>();
                }
            });


            _ = LoadCampaignsAsync();

            InitializeCounts();


        }

        private async void InitializeCounts()
        {
            List<Campaign> campaignsData = await _campaignService.GetAllCampaignsAsync(new CampaignIncludeOptions { IncludeDonationRecord = true });
            List<DonationRecord> donationsData = await _donationRecordService.GetAllCampaignsAsync();
            ProjectsFunded = campaignsData.FindAll(item => item.DonationRaised > 0).Count;
            Raised = donationsData.Sum(item => item.Amount);
            Donors = donationsData.Count;
            CampaignCount = campaignsData.Count;
        }

        private async Task LoadCampaignsAsync()
        {
            try
            {
                var campaigns = await _campaignService.GetAllCampaignsAsync(new CampaignIncludeOptions { IncludeDonationRecord = true});

                Campaigns.Clear();
                foreach (var campaign in campaigns)
                {
                    Campaigns.Add(campaign);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading campaigns: {ex.Message}");
            }
        }

        public async void ApplyNavigationParameter(object parameter)
        {
            if (parameter == null) return;
            Dictionary<string, object> props = parameter as Dictionary<string, object>;

            SuccessMessage = props["message"] as string;
            ShowMessage = (bool) props["success"];

        }
    }
}
