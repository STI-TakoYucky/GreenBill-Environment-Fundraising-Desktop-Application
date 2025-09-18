using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.MVVM.Model
{
    public class Donation
    {
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public ObjectId CampaignId { get; set; }
        public decimal Amount { get; set; }
        public string StripePaymentIntentId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
