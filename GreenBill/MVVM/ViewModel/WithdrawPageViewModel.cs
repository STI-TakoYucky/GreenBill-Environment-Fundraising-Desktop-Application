using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using LiveCharts;
using MongoDB.Bson;
using Stripe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private List<WithdrawalRecord> _withdrawalRecords;
        public List<WithdrawalRecord> WithdrawalRocords
        {
            get => _withdrawalRecords;
            set
            {
                _withdrawalRecords = value;
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

        private ObservableCollection<string> _bankAccounts;
        public ObservableCollection<string> BankAccounts
        {
            get => _bankAccounts;
            set
            {
                _bankAccounts = value;
                OnPropertyChanged();
            }
        }

        private string _selectedBankAccount;
        public string SelectedBankAccount
        {
            get => _selectedBankAccount;
            set
            {
                _selectedBankAccount = value;
                OnPropertyChanged();
            }
        }

        private string _accountNumber;
        public string AccountNumber
        {
            get => _accountNumber;
            set
            {
                _accountNumber = value;
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
                UpdateAmountAfterFee();
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
                UpdateAmountAfterFee();
            }
        }

        private decimal _amountAfterFee;
        public decimal AmountAfterFee
        {
            get => _amountAfterFee;
            set
            {
                _amountAfterFee = value;
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
            _withdrawalRecordService = withdrawalRecordService;
            Withdraw = new RelayCommand(o => RequestWithdrawal());

            // Initialize bank account types
            BankAccounts = new ObservableCollection<string>
            {
                "GCash",
                "Maya",
                "BDO",
                "BPI",
                "Metrobank",
                "UnionBank",
                "Security Bank",
                "Landbank",
                "PNB"
            };
        }

        private void UpdateAmountAfterFee()
        {
            const decimal processingFee = 25.00m;
            AmountAfterFee = Amount > processingFee ? Amount - processingFee : 0;
        }

        public async void RequestWithdrawal()
        {
            if (WithdrawableAmount == 0)
            {
                MessageBox.Show("There is nothing to withdraw");
                return;
            }
            if (Amount > WithdrawableAmount)
            {
                MessageBox.Show("Amount to be withdrawn should be less than or equal to withdrawable amount");
                return;
            }
            if (Amount <= 0)
            {
                MessageBox.Show("Please enter a valid amount");
                return;
            }
            if (string.IsNullOrEmpty(SelectedBankAccount))
            {
                MessageBox.Show("Please select a bank account type");
                return;
            }
            if (string.IsNullOrWhiteSpace(AccountNumber))
            {
                MessageBox.Show("Please enter your account number");
                return;
            }

            var result = MessageBox.Show("Are you sure that all information you entered is correct?", "confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No) return;
   
            IsLoading = true;
            User user = _userSessionService.CurrentUser;

            await _withdrawalRecordService.Create(new WithdrawalRecord
            {
                CampaignId = SelectedCampaign.Id,
                Amount = this.Amount,
                BankAccount = SelectedBankAccount,
                AccountNumber = AccountNumber
            });

            refreshPage();
            IsLoading = false;

            MessageBox.Show("Withdrawal request submitted successfully.");
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

            WithdrawalRocords = SelectedCampaign.WithdrawalRecord;

            Debug.WriteLine(WithdrawalRocords.Count);

            // Get the withdrawed amount
            var withdrawedAmount = SelectedCampaign.WithdrawalRecord.Sum(item => item.Amount);
            WithdrawedAmount = withdrawedAmount;

            // Get the withdrawable amount
            var donationsAmount = SelectedCampaign.DonationRecord.Sum(item => item.Amount);
            WithdrawableAmount = (long)donationsAmount - WithdrawedAmount;

            SelectedBankAccount = BankAccounts.FirstOrDefault();

            IsLoading = false;
        }

        public async void refreshPage()
        {
            SelectedCampaign = await _campaignService.GetCampaignByIdAsync(CampaignId.ToString(), new CampaignIncludeOptions
            {
                IncludeWithdrawalRecord = true,
                IncludeDonationRecord = true
            });

            WithdrawalRocords = SelectedCampaign.WithdrawalRecord;

            Debug.WriteLine(WithdrawalRocords.Count);

            // Get the withdrawed amount
            var withdrawedAmount = SelectedCampaign.WithdrawalRecord.Sum(item => item.Amount);
            WithdrawedAmount = withdrawedAmount;

            // Get the withdrawable amount
            var donationsAmount = SelectedCampaign.DonationRecord.Sum(item => item.Amount);
            WithdrawableAmount = (long)donationsAmount - WithdrawedAmount;

            // Reset fields after withdrawal
            Amount = 0;
            AccountNumber = string.Empty;
        }
    }
}