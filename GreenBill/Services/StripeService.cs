using GreenBill.IServices;
using GreenBill.MVVM.Model;
using Stripe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.Services
{
    public class StripeService : IStripeService
    {
        private readonly IUserService _userService;

        public StripeService(IUserService userService) 
        {
            _userService = userService;
     
        }

        public async Task<bool> PayoutFundsAsync(string stripeAccountId, long amountInCents, string bankAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(stripeAccountId))
                {
                    Console.WriteLine("No Stripe account ID provided");
                    return false;
                }

                if (string.IsNullOrEmpty(bankAccountId))
                {
                    Console.WriteLine("No bank account selected");
                    return false;
                }

                var payoutService = new PayoutService();
                var payoutOptions = new PayoutCreateOptions
                {
                    Amount = amountInCents,
                    Currency = "usd",
                    Method = "instant"
                };

                var requestOptions = new RequestOptions();
                requestOptions.StripeAccount = stripeAccountId;

                var payout = await payoutService.CreateAsync(payoutOptions, requestOptions);

                if (payout != null && !string.IsNullOrEmpty(payout.Id))
                {
                    Console.WriteLine($"Payout successful: {payout.Id}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Payout failed");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to process payout: {ex.Message}");
                return false;
            }
        }

        public async Task<Payout> GetPayoutDetailsAsync(string stripeAccountId, string payoutId)
        {
            try
            {
                var payoutService = new PayoutService();

                var requestOptions = new RequestOptions
                {
                    StripeAccount = stripeAccountId
                };

                var payout = await payoutService.GetAsync(payoutId, null, requestOptions);

                return payout;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve payout details: {ex.Message}");
                return null;
            }
        }
        public async Task CreateConnectAccountAsync(User user)
        {
            try
            {
                var accountService = new AccountService();
                var accountOptions = new AccountCreateOptions
                {
                    Type = "express",
                    Country = "US",
                    Email = user.Email,
                };

                var stripeAccount = await accountService.CreateAsync(accountOptions);


                var linkService = new AccountLinkService();
                var linkOptions = new AccountLinkCreateOptions
                {
                    Account = stripeAccount.Id,
                    RefreshUrl = "https://indiaesevakendra.in/wp-content/uploads/2020/08/Paymentsuccessful21-768x427.png",
                    ReturnUrl = "https://indiaesevakendra.in/wp-content/uploads/2020/08/Paymentsuccessful21-768x427.png",
                    Type = "account_onboarding"
                };

  

                var accountLink = await linkService.CreateAsync(linkOptions);

                user.StripeAccountId = stripeAccount.Id;
                user.VerificationStatus = "pending_onboarding";
                user.CanReceiveFunds = true;

                await _userService.UpdateUserAsync(user.Id, user);

                Console.WriteLine("REDIRECTING");
                Process.Start(new ProcessStartInfo
                {
                    FileName = accountLink.Url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create Stripe account for user {user.Email}: {ex.Message}");

                user.StripeAccountId = null;
                user.VerificationStatus = "failed";
                user.CanReceiveFunds = false;

                await _userService.UpdateUserAsync(user.Id, user);

            }
        }

        public async Task<List<dynamic>> GetConnectedBankAccountsAsync(string stripeAccountId)
        {
            try
            {
                var accountService = new AccountService();

                // Get the connected account with its external accounts
                var account = await accountService.GetAsync(stripeAccountId);

                var externalAccounts = new List<dynamic>();

                if (account?.ExternalAccounts?.Data != null)
                {
                    foreach (var externalAccount in account.ExternalAccounts.Data)
                    {
                        externalAccounts.Add(externalAccount);
                    }
                }

                return externalAccounts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve bank accounts for Stripe account {stripeAccountId}: {ex.Message}");
                return new List<dynamic>();
            }
        }
        public async Task<List<dynamic>> GetFormattedBankAccountsAsync(string stripeAccountId)
        {
            try
            {
                var externalAccounts = await GetConnectedBankAccountsAsync(stripeAccountId);

                var formattedAccounts = new List<dynamic>();

                foreach (var account in externalAccounts)
                {
                    var accountDict = new Dictionary<string, object>
                    {
                        { "Id", account.Id },
                        { "Last4", account.Last4 ?? "N/A" },
                        { "Type", account.Object ?? "Unknown" }
                    };

                    // Handle both BankAccount and Card types
                    if (account is BankAccount bankAccount)
                    {
                        accountDict["BankName"] = bankAccount.BankName ?? "Unknown";
                        accountDict["AccountHolderName"] = bankAccount.AccountHolderName ?? "Unknown";
                        accountDict["Status"] = bankAccount.Status ?? "unknown";
                    }
                    else if (account is Card card)
                    {
                        accountDict["Brand"] = card.Brand ?? "Unknown";
                        accountDict["Status"] = "active";
                    }

                    formattedAccounts.Add(accountDict);
                }

                return formattedAccounts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to format bank accounts: {ex.Message}");
                return new List<dynamic>();
            }
        }
    }
}
