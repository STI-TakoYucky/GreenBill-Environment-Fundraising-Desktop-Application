using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

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
        public string Status { get; set; } = "in review";

        [BsonIgnore]
        public User User { get; set; }
        [BsonIgnore]
        public List<DonationRecord> DonationRecord { get; set; }
        [BsonIgnore]
        public List<CampaignUpdate> CampaignUpdate { get; set; }
        [BsonIgnore]
        public string TotalAmountRaised { get; set; }
        
        [BsonIgnore]
        public string DonationsCount {  get; set; }
        [BsonIgnore]
        public string Percentage {  get; set; }
    }
}