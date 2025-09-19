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

        // Donation form properties
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

        private string _customAmount = "";
        public string CustomAmount
        {
            get => _customAmount;
            set
            {
                _customAmount = value;
                OnPropertyChanged();
                // Parse custom amount and update SelectedAmount
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

        // Commands
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
                CustomAmount = ""; // Clear custom amount when selecting predefined amount
            }
        }

        public async void HandleDonation()
        {
            try
            {
                // Validate required fields
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

                // Convert decimal to cents (Stripe uses cents)
                long donationAmount = (long)(SelectedAmount * 100);
                long platformFee = (long)(donationAmount * 0.01); // 1% platform fee
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
               new CampaignIncludeOptions { IncludeUser = true }
            );
        }
    }
}