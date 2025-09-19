using GreenBill.Core;
using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.Services;
using MongoDB.Bson;
using MongoDB.Driver.Core.Authentication;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public ICommand NavigateBack { get; set; }
        public ICommand CompleteDonation {  get; set; }

        public DonationPageViewModel(INavigationService navService, ICampaignService campaignService, IDonationRecordService donationRecordService)
        {
            Navigation = navService;
            _campaignService = campaignService;
            _donationRecordService = donationRecordService;
            NavigateBack = new RelayCommand(o => Navigation.NavigateBack(), o => Navigation.CanNavigateBack);
            CompleteDonation = new RelayCommand(o => HandleDonation());
            _donationRecordService = donationRecordService;
        }

        public async void HandleDonation()
        {
            try
            {
                string connectedAccountId = "acct_1S8coYHsRmSm7fTN"; 
                ObjectId campaignId = ObjectId.Parse("68cb8c7cd6138683d15bd14e");
                long donationAmount = 10000; 
                long platformFee = 100;
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
                            Name = $"Donation to Campaign {campaignId}",
                        },
                    },
                    Quantity = 1,
                },
            },
                    Mode = "payment",
                    SuccessUrl = "https://www.google.com", 
                    CancelUrl = "https://www.github.com",
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
                    { "platform_fee", platformFee.ToString() }
                }
                    }
                };

                var session = await sessionService.CreateAsync(sessionOptions);

                await SaveDonationToDatabase(new DonationRecord
                {
                    PaymentIntentId = session.PaymentIntentId,
                    CheckoutSessionId = session.Id,
                    CampaignId = campaignId,
                    ConnectedAccountId = connectedAccountId,
                    Amount = donationAmount,
                    PlatformFee = platformFee,
                    OrganizerAmount = organizerAmount,
                    Status = "pending_payment",
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
        }
    }
}
