using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using MongoDB.Bson;
using Stripe;
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
        private bool _showSuccessMessage = false;
        public bool ShowSuccessMessage
        {
            get => _showSuccessMessage;
            set
            {
                _showSuccessMessage = value;
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

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private IStripeService _stripeService;
        private IUserSessionService _userSessionService;
        private INavigationService _navigationService;
        private IWithdrawalRecordService _withdrawalRecordService;
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

        private long _pending;
        public long Pending
        {
            get => _pending;
            set
            {
                _pending = value;
                OnPropertyChanged();
            }
        }

        private long _amount;
        public long Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }

        public ICommand Withdraw { get; }

        public WithdrawPageViewModel(INavigationService navigationService, ICampaignService campaignService, IUserSessionService userSessionService, IStripeService stripeService, IWithdrawalRecordService withdrawalRecordService)
        {
            Navigation = navigationService;
            BackCommand = new RelayCommand(op => Navigation.NavigateBack(), o => Navigation.CanNavigateBack);
            _campaignService = campaignService;
            _userSessionService = userSessionService;
            _stripeService = stripeService;
            BankAccounts = new ObservableCollection<dynamic>();
            _withdrawalRecordService = withdrawalRecordService;
            Withdraw = new RelayCommand(o => RequestWithdrawal());
        }

        public async void RequestWithdrawal()
        {
            IsLoading = true;
            User user = _userSessionService.CurrentUser;
           
            Debug.WriteLine($"Amount: {Amount * 100}");
            Debug.WriteLine($"Bank Number: {SelectedBankAccount}");
            var payout = await _stripeService.PayoutFundsAsync(user.StripeAccountId, Amount * 100, SelectedBankAccount.Id);
            Debug.WriteLine($"PAYOUT: {payout}");
            if (payout)
            {
                await _withdrawalRecordService.Create(new WithdrawalRecord { CampaignId = SelectedCampaign.Id, Amount = this.Amount });
            }
            IsLoading = false;
        }

        public async void ApplyNavigationParameter(object parameter)
        {
            IsLoading = true;
            User user = _userSessionService.CurrentUser;
            if (parameter == null) return;
            CampaignId = (ObjectId)parameter;
            SelectedCampaign = await _campaignService.GetCampaignByIdAsync(CampaignId.ToString(), new CampaignIncludeOptions
            {
                IncludeWithdrawalRecord = true,
                IncludeDonationRecord = true
            });

            var balanceService = new BalanceService();
            var requestOptions = new RequestOptions { StripeAccount = user.StripeAccountId };
            var balance = await balanceService.GetAsync(requestOptions);


            // Get the withdrawed amount
            var withdrawedAmount = SelectedCampaign.WithdrawalRecord.Sum(item => item.Amount);
            WithdrawedAmount = withdrawedAmount;

            // Get the withdrawable amount
            var donationsAmount = SelectedCampaign.DonationRecord.Sum(item => item.Amount / 100);
            WithdrawableAmount = balance.Available[0].Amount;

            Pending = balance.Pending[0].Amount / 100;


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
            IsLoading = false;
        }
    }
}