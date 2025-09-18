using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.MVVM.Model
{
    public class DonationRecord
    {
        public ObjectId Id { get; set; }
        public string PaymentIntentId { get; set; }
        public ObjectId CampaignId { get; set; }
        public string ConnectedAccountId { get; set; }
        public string CheckoutSessionId { get; set; }
        public long Amount { get; set; }
        public long PlatformFee { get; set; }
        public long OrganizerAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
