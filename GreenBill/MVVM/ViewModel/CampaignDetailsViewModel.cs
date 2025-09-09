using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.MVVM.View.CampaignDetailsTabs;
using GreenBill.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel
{
    public class CampaignDetailsViewModel : Core.ViewModel, INotifyPropertyChanged, INavigatableService, INavigationAware
    {
        private INavigationService _navigationService;
        public bool ShowNavigation => false;

        public INavigationService Navigation
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }
        private UserControl _currentTabContent;
        private string _selectedTab = "DETAILS";

        private readonly Details _detailsTab = new Details();
        private readonly Donors _donorsTab = new Donors();
        private readonly Updates _updatesTab = new Updates();
        private readonly SupportingDocuments _supportingDocumentTab = new SupportingDocuments();

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

        public CampaignDetailsViewModel(INavigationService navService, ICampaignService campaignService)
        {
            _navigationService = navService;
            _campaignService = campaignService;
            _currentTabContent = _detailsTab;

            SelectDetailsCommand = new RelayCommand(o => SelectTab("DETAILS"));
            SelectDonorsCommand = new RelayCommand(o => SelectTab("DONORS"));
            SelectUpdatesCommand = new RelayCommand(o => SelectTab("UPDATES"));
            SelectSupportingDocumentsCommand = new RelayCommand(o => SelectTab("SUPPORTING DOCUMENTS"));
            GoBackCommand = new RelayCommand(o => Navigation.NavigateTo<UserCampaignsViewModel>());
            GoToSupportingDocumentsPage = new RelayCommand(campaign_id => Navigation.NavigateTo<SupportingDocumentsPageViewModel>(campaign_id));
        }

        public UserControl CurrentTabContent
        {
            get => _currentTabContent;
            set
            {
                _currentTabContent = value;
                OnPropertyChanged();
            }
        }

        public string SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsDetailsSelected));
                OnPropertyChanged(nameof(IsDonorsSelected));
                OnPropertyChanged(nameof(IsUpdatesSelected));
                OnPropertyChanged(nameof(IsSupportingDocumentsSelected));
            }
        }

        public bool IsDetailsSelected => SelectedTab == "DETAILS";
        public bool IsDonorsSelected => SelectedTab == "DONORS";
        public bool IsUpdatesSelected => SelectedTab == "UPDATES";
        public bool IsSupportingDocumentsSelected => SelectedTab == "SUPPORTING DOCUMENTS";

        public ICommand SelectDetailsCommand { get; }
        public ICommand SelectUpdatesCommand { get; }
        public ICommand SelectDonorsCommand { get; }
        public ICommand SelectSupportingDocumentsCommand { get; }
        public ICommand GoToSupportingDocumentsPage {  get; }
        public ICommand GoBackCommand { get; }

        private void SelectTab(string tabName)
        {
            SelectedTab = tabName;

            switch (tabName)
            {
                case "DETAILS":
                    CurrentTabContent = _detailsTab;
                    break;
                case "DONORS":
                    CurrentTabContent = _donorsTab;
                    break;
                case "UPDATES":
                    CurrentTabContent = _updatesTab;
                    break;
                case "SUPPORTING DOCUMENTS":
                    CurrentTabContent = _supportingDocumentTab;
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void ApplyNavigationParameter(object parameter)
        {
            if (parameter == null) return;
            var id = parameter.ToString();
            SelectedCampaign = await _campaignService.GetCampaignByIdAsync(id);
        }
    }
}