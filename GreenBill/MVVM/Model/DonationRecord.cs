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
        public ObjectId UserId { get; set; }
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
        public string Email { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime CreatedAt { get; set; }

        [BsonIgnore]
        public string FullName
        {
            get { return this.FirstName + " " + this.LastName; }
        }
        [BsonIgnore]
        public string DisplayName
        {
            get
            {
                if (this.IsAnonymous)
                {
                    return "Anonymous";
                }
                else
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

        [BsonIgnore]
        public string DaySpan
        {
            get
            {
                var timeSpan = DateTime.Now - CreatedAt;

                if (timeSpan.TotalDays >= 365)
                {
                    int years = (int)(timeSpan.TotalDays / 365);
                    return years == 1 ? "1 year ago" : $"{years} years ago";
                }
                else if (timeSpan.TotalDays >= 30)
                {
                    int months = (int)(timeSpan.TotalDays / 30);
                    return months == 1 ? "1 month ago" : $"{months} months ago";
                }
                else if (timeSpan.TotalDays >= 7)
                {
                    int weeks = (int)(timeSpan.TotalDays / 7);
                    return weeks == 1 ? "1 week ago" : $"{weeks} weeks ago";
                }
                else if (timeSpan.TotalDays >= 1)
                {
                    int days = (int)timeSpan.TotalDays;
                    return days == 1 ? "1 day ago" : $"{days} days ago";
                }
                else if (timeSpan.TotalHours >= 1)
                {
                    int hours = (int)timeSpan.TotalHours;
                    return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
                }
                else if (timeSpan.TotalMinutes >= 1)
                {
                    int minutes = (int)timeSpan.TotalMinutes;
                    return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
                }
                else
                {
                    return "Just now";
                }
            }
        }
    




    }

}