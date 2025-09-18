using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.Services
{
    public class StripeService
    {
        public StripeService() 
        {
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_API_KEY");
        }

    }
}
