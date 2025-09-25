using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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
        public ObjectId UserId {  get; set; }
        public string PaymentIntentId { get; set; }
        public ObjectId CampaignId { get; set; }
        public string ConnectedAccountId { get; set; }
        public string CheckoutSessionId { get; set; }
        public long Amount { get; set; }
        public long PlatformFee { get; set; }
        public long OrganizerAmount { get; set; }
        public string Status { get; set; }
        public string FirstName { get; set; } = "Anonymous";
        public string LastName { get; set; } = "Supporter";
        public string Email {  get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime CreatedAt { get; set; }

        [BsonIgnore]
        public string FullName
        {
            get { return this.FirstName + " " + this.LastName; }
        }

        public string DisplayName
        {
            get
            {
                if (this.IsAnonymous)
                {
                    return "Anonymous";
                }else
                {
                    return this.FirstName + " " + this.LastName;
                }
            }
        }

        [BsonIgnore]
        public decimal RealAmount
        {
            get { return Amount / 100m; }
        }

        [BsonIgnore]
        public decimal RealPlatformFee
        {
            get { return PlatformFee / 100m; }
        }

        [BsonIgnore]
        public decimal RealOrganizerAmount
        {
            get { return OrganizerAmount / 100m; }
        }


    }
}
