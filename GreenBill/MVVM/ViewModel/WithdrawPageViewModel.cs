using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel
{
    public class WithdrawPageViewModel : Core.ViewModel, INavigatableService
    {
        private IStripeService _stripeService;
        private IUserSessionService _userSessionService;
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

        private ICampaignService _campaignService;
        private ObjectId CampaignId { get; set; }

        private Campaign _campaign;
        public Campaign SelectedCampaign
        {
            get => _campaign;
            set
            {
                _campaign = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<dynamic> _bankAccounts;
        public ObservableCollection<dynamic> BankAccounts
        {
            get => _bankAccounts;
            set
            {
                _bankAccounts = value;
                OnPropertyChanged();
            }
        }

        private dynamic _selectedBankAccount;
        public dynamic SelectedBankAccount
        {
            get => _selectedBankAccount;
            set
            {
                _selectedBankAccount = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackCommand { get; }

        private long _withdrawableAmount;
        public long WithdrawableAmount
        {
            get => _withdrawableAmount;
            set
            {
                _withdrawableAmount = value;
                OnPropertyChanged();
            }
        }

        private long _withdrawedAmount;
        public long WithdrawedAmount
        {
            get => _withdrawedAmount;
            set
            {
                _withdrawedAmount = value;
                OnPropertyChanged();
            }
        }

        public WithdrawPageViewModel(INavigationService navigationService, ICampaignService campaignService, IUserSessionService userSessionService, IStripeService stripeService)
        {
            Navigation = navigationService;
            BackCommand = new RelayCommand(op => Navigation.NavigateBack(), o => Navigation.CanNavigateBack);
            _campaignService = campaignService;
            _userSessionService = userSessionService;
            _stripeService = stripeService;
            BankAccounts = new ObservableCollection<dynamic>();
        }

        public async void ApplyNavigationParameter(object parameter)
        {
            if (parameter == null) return;
            CampaignId = (ObjectId)parameter;
            SelectedCampaign = await _campaignService.GetCampaignByIdAsync(CampaignId.ToString(), new CampaignIncludeOptions
            {
                IncludeWithdrawalRecord = true,
                IncludeDonationRecord = true
            });

            // Get the withdrawed amount
            var withdrawedAmount = SelectedCampaign.WithdrawalRecord.Sum(item => item.Amount);
            WithdrawedAmount = withdrawedAmount;

            // Get the withdrawable amount
            var donationsAmount = SelectedCampaign.DonationRecord.Sum(item => item.Amount / 100);
            WithdrawableAmount = donationsAmount - withdrawedAmount;

            User user = _userSessionService.CurrentUser;
            var bankAccounts = await _stripeService.GetConnectedBankAccountsAsync(user.StripeAccountId);

            // Bind bank accounts to the observable collection
            BankAccounts.Clear();
            foreach (var account in bankAccounts)
            {
                BankAccounts.Add(account);
            }

            // Select the first account by default
            if (BankAccounts.Count > 0)
            {
                SelectedBankAccount = BankAccounts[0];
            }
        }
    }
}