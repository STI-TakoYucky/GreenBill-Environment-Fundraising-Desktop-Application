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
            StripeConfiguration.ApiKey = "sk_test_51RNlS7H0KVHxP8CWywsphLYId1CavpCnpDW9BXm2yycKudwQQn1kmI6zPQsOHQuQUDXeLHo5AJZBfiP2i3lObxbR00ha4k1FSj";
        }

    }
}
