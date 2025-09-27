using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
namespace GreenBill.MVVM.ViewModel
{
    public class FundraisingDetailsViewModel : Core.ViewModel, INavigatableService
    {
        private INavigationService _navigationService;

        private IDonationRecordService _donationRecordService;
        private ObservableCollection<DonationRecord> _donations;
        public ObservableCollection<DonationRecord> Donations
        {
            get => _donations;
            set
            {
                _donations = value;
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

        private string _total_donation_raised;
        public string TotalDonationRaised
        {
            get => _total_donation_raised;
            set
            {
                _total_donation_raised = value;
                OnPropertyChanged();
            }
        }

        private string _percentage;
        public string Percentage
        {
            get => _percentage;
            set
            {
                _percentage = value;
                OnPropertyChanged();
            }
        }

        private string _campaignId;
        public string CampaignId
        {
            get => _campaignId;
            set
            {
                _campaignId = value;
                OnPropertyChanged();
            }
        }
        private ICampaignService _campaignService;

        private Campaign _selectedCampaign;
        public Campaign SelectedCampaign
        {
            get => _selectedCampaign;
            set
            {
                _selectedCampaign = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateBack { get; set; }
        public ICommand NavigateToDonationPage { get; set; }

        public FundraisingDetailsViewModel(INavigationService navService, ICampaignService campaignService, IDonationRecordService donationRecordService)
        {
            Navigation = navService;
            _campaignService = campaignService;
            InitializeCommands();
            _donationRecordService = donationRecordService;
        }

        public void InitializeCommands()
        {
            NavigateBack = new RelayCommand(o => Navigation.NavigateBack(), o => Navigation.CanNavigateBack);
            NavigateToDonationPage = new RelayCommand(o =>
            {
                Debug.WriteLine($"API KEY: {Environment.GetEnvironmentVariable("STRIPE_API_KEY")}");
                Navigation.NavigateTo<DonationPageViewModel>(o);
            });
        }
        public async void ApplyNavigationParameter(object parameter)
        {
            if (parameter == null) return;
            var id = parameter.ToString();
            SelectedCampaign = await _campaignService.GetCampaignByIdAsync(
                id,
                new CampaignIncludeOptions { IncludeUser = true, IncludeDonationRecord = true }
             );

            //var total = $"${(SelectedCampaign.DonationRecord?.Sum(item => item.RealAmount) ?? 0):N2} USD raised";
            //TotalDonationRaised = total;
            //var percentage = ((SelectedCampaign.DonationRecord?.Sum(item => item.RealAmount) ?? 0) / SelectedCampaign.DonationGoal) * 100;
            //Percentage = $"{percentage:N0}% of {SelectedCampaign.DonationGoal:N2} goal";
        }
    }
}
