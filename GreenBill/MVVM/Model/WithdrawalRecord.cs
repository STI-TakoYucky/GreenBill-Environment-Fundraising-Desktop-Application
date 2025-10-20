using MongoDB.Bson;
using System;

namespace GreenBill.MVVM.Model
{
    public class WithdrawalRecord
    {
        public ObjectId Id { get; set; }
        public ObjectId CampaignId { get; set; }
        public string Status { get; set; }
        public long Amount { get; set; }
        public long ProcessingFee { get; set; }
        public long Remarks { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
