using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using MongoDB.Bson;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GreenBill.MVVM.ViewModel
{
    public class DonationPageViewModel : Core.ViewModel, INavigatableService
    {
        private INavigationService _navigationService;
        private IDonationRecordService _donationRecordService;
        private ICampaignService _campaignService;
      

        public INavigationService Navigation
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

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

        private decimal _selectedAmount = 10;
        public decimal SelectedAmount
        {
            get => _selectedAmount;
            set
            {
                _selectedAmount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CustomAmount));
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

        private string _customAmount = "";
        public string CustomAmount
        {
            get => _customAmount;
            set
            {
                _customAmount = value;
                OnPropertyChanged();
                if (decimal.TryParse(value, out decimal amount) && amount > 0)
                {
                    _selectedAmount = amount;
                    OnPropertyChanged(nameof(SelectedAmount));
                }
            }
        }

        private string _firstName = "";
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        private string _lastName = "";
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        private string _emailAddress = "";
        public string EmailAddress
        {
            get => _emailAddress;
            set
            {
                _emailAddress = value;
                OnPropertyChanged();
            }
        }

        private bool _isAnonymous = false;
        public bool IsAnonymous
        {
            get => _isAnonymous;
            set
            {
                _isAnonymous = value;
                OnPropertyChanged();
            }
        }

        private string _selectAmountError;
        public string SelectedAmountError
        {
            get => _selectAmountError;
            set
            {
                _selectAmountError = value;
                OnPropertyChanged();
            }
        }

        private string _firstNameError;
        public string FirstNameError
        {
            get => _firstNameError;
            set
            {
                _firstNameError = value;
                OnPropertyChanged();
            }
        }

        private string _lastNameError;
        public string LastNameError
        {
            get => _lastNameError;
            set
            {
                _lastNameError = value;
                OnPropertyChanged();
            }
        }

        private string _emailError;
        public string EmailError
        {
            get => _emailError;
            set
            {
                _emailError = value;
                OnPropertyChanged();
            }
        }

        public bool _hasErrors = false;
        public bool HasErrors
        {
            get => _hasErrors;
            set
            {
                _hasErrors = value;
                OnPropertyChanged();
            }
        }




        public ICommand NavigateBack { get; set; }
        public ICommand CompleteDonation { get; set; }
        public ICommand SelectAmountCommand { get; set; }

        public DonationPageViewModel(INavigationService navService, ICampaignService campaignService, IDonationRecordService donationRecordService)
        {
            Navigation = navService;
            _campaignService = campaignService;
            _donationRecordService = donationRecordService;

            NavigateBack = new RelayCommand(o => Navigation.NavigateBack(), o => Navigation.CanNavigateBack);
            CompleteDonation = new RelayCommand(o => HandleDonation());
            SelectAmountCommand = new RelayCommand(SelectAmount);
        }

        private void SelectAmount(object parameter)
        {
            if (parameter != null && decimal.TryParse(parameter.ToString(), out decimal amount))
            {
                SelectedAmount = amount;
                CustomAmount = "";
            }
        }

        public async void HandleDonation()
        {
            try
            {
                if (SelectedAmount <= 0)
                {
                    MessageBox.Show("Please select a valid donation amount.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(FirstName) && !IsAnonymous)
                {
                    MessageBox.Show("Please enter your first name or check 'Make this donation anonymous'.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(LastName) && !IsAnonymous)
                {
                    MessageBox.Show("Please enter your last name or check 'Make this donation anonymous'.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(EmailAddress))
                {
                    MessageBox.Show("Please enter your email address.");
                    return;
                }

                string connectedAccountId = SelectedCampaign.User.StripeAccountId;
                ObjectId campaignId = SelectedCampaign.Id;


                long donationAmount = (long)(SelectedAmount * 100);
                long platformFee = (long)(donationAmount * 0.01); 
                long organizerAmount = donationAmount - platformFee;

                var sessionService = new Stripe.Checkout.SessionService();
                var sessionOptions = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = donationAmount,
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"Donation to {SelectedCampaign.Title ?? "Campaign"}",
                                },
                            },
                            Quantity = 1,
                        },
                    },
                    Mode = "payment",
                    SuccessUrl = "https://indiaesevakendra.in/wp-content/uploads/2020/08/Paymentsuccessful21-768x427.png",
                    CancelUrl = "https://docs.memberstack.com/hc/article_attachments/16017242490267",
                    PaymentIntentData = new SessionPaymentIntentDataOptions
                    {
                        TransferData = new SessionPaymentIntentDataTransferDataOptions
                        {
                            Destination = connectedAccountId,
                            Amount = organizerAmount
                        },
                        Metadata = new Dictionary<string, string>
                        {
                            { "campaign_id", campaignId.ToString() },
                            { "donation_type", "campaign_donation" },
                            { "platform_fee", platformFee.ToString() },
                            { "donor_email", EmailAddress }
                        }
                    }
                };

                var session = await sessionService.CreateAsync(sessionOptions);

                await SaveDonationToDatabase(new DonationRecord
                {
                    PaymentIntentId = session.PaymentIntentId,
                    CheckoutSessionId = session.Id,
                    FirstName = FirstName,
                    LastName =  LastName,
                    IsAnonymous = IsAnonymous,
                    Email = EmailAddress,
                    CampaignId = campaignId,
                    ConnectedAccountId = connectedAccountId,
                    Amount = donationAmount,
                    PlatformFee = platformFee,
                    OrganizerAmount = organizerAmount,
                    Status = "done",
                    CreatedAt = DateTime.UtcNow
                });

                Process.Start(new ProcessStartInfo
                {
                    FileName = session.Url,
                    UseShellExecute = true
                });
            }
            catch (StripeException stripeEx)
            {
                MessageBox.Show($"Stripe Error: {stripeEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing donation: {ex.Message}");
            }
            finally
            {
                Navigation.NavigateTo<FundraisingDetailsViewModel>(SelectedCampaign.Id.ToString());
            }
        }

        private async Task SaveDonationToDatabase(DonationRecord donation)
        {
            await _donationRecordService.Create(donation);
        }

        public async void ApplyNavigationParameter(object parameter)
        {
            if (parameter == null) return;
            var id = parameter.ToString();
            SelectedCampaign = await _campaignService.GetCampaignByIdAsync(
               id,
               new CampaignIncludeOptions { IncludeUser = true, IncludeDonationRecord = true }
            );
            var total = $"${(SelectedCampaign.DonationRecord?.Sum(item => item.RealAmount) ?? 0):N2} USD raised";
            TotalDonationRaised = total;
            var percentage = ((SelectedCampaign.DonationRecord?.Sum(item => item.RealAmount) ?? 0) / SelectedCampaign.DonationGoal) * 100;
            Percentage = $"{percentage:N0}% of {SelectedCampaign.DonationGoal:N2} goal";
        }

        public void validateInputs()
        {
            HasErrors = false;
            if(SelectedAmount <= 0)
            {
                SelectedAmountError = "";
                SelectedAmountError = "Please enter a valid amount";
                HasErrors = true;
            } 
            if(!IsAnonymous && string.IsNullOrEmpty(FirstName))
            {
                FirstNameError = "";
                FirstNameError = "This field is required";
                HasErrors = true;
            }
            if (!IsAnonymous && string.IsNullOrEmpty(LastName))
            {
                LastNameError = "";
                LastNameError = "This field is required";
                HasErrors = true;
            }
            if (!IsAnonymous && string.IsNullOrEmpty(EmailAddress))
            {
                EmailError = "";
                EmailError = "This field is required";
                HasErrors = true;
            }

        }
    }
}