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
            StripeConfiguration.ApiKey = "sk_test_51RNlS7H0KVHxP8CWywsphLYId1CavpCnpDW9BXm2yycKudwQQn1kmI6zPQsOHQuQUDXeLHo5AJZBfiP2i3lObxbR00ha4k1FSj";
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
                    RefreshUrl = "https://google.com",
                    ReturnUrl = "https://google.com",
                    Type = "account_onboarding"
                };

                user.StripeAccountId = stripeAccount.Id;
                user.VerificationStatus = "pending_onboarding";
                user.CanReceiveFunds = true;

                await _userService.UpdateUserAsync(user.Id, user);

                var accountLink = await linkService.CreateAsync(linkOptions);
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
    }
}
