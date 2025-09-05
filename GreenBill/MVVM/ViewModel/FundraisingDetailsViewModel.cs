using GreenBill.Core;
using GreenBill.Services;
using GreenBill.IServices;
using System.Windows.Input;
using GreenBill.MVVM.Model;
namespace GreenBill.MVVM.ViewModel
{
    public class FundraisingDetailsViewModel : Core.ViewModel, INavigatableService
    {
        private INavigationService _navigationService;


        public INavigationService Navigation
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
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

        public ICommand NavigateToHome { get; set; }

        public FundraisingDetailsViewModel(INavigationService navService, ICampaignService campaignService)
        {
            Navigation = navService;
            _campaignService = campaignService;
            InitializeCommands();
        }

        public void InitializeCommands()
        {
            NavigateToHome = new RelayCommand(o => Navigation.NavigateTo<HomePageViewModel>());
        }
        public async void ApplyNavigationParameter(object parameter)
        {
            if (parameter == null) return;
            var id = parameter.ToString();
            SelectedCampaign = await _campaignService.GetCampaignByIdAsync(id);
        }
    }
}
