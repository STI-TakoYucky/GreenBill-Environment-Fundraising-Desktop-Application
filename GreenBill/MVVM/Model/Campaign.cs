using MongoDB.Bson;
using System;

namespace GreenBill.MVVM.Model
{
    public class Campaign
    {
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; } 
        public string Category { get; set; }
        public decimal DonationGoal { get; set; }
        public decimal DonationRaised { get; set; }
        public byte[] Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
         public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "verified";

    }
}