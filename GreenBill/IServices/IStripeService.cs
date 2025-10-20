using GreenBill.MVVM.Model;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.IServices
{
    public interface IStripeService
    {
        Task CreateConnectAccountAsync(User user);
        Task<List<dynamic>> GetConnectedBankAccountsAsync(string stripeAccountId);
        Task<List<dynamic>> GetFormattedBankAccountsAsync(string stripeAccountId);

    }
}
