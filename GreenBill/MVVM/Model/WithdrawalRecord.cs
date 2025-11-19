using MongoDB.Bson;
using System;

namespace GreenBill.MVVM.Model
{
    public class WithdrawalRecord
    {
        public ObjectId Id { get; set; }
        public ObjectId CampaignId { get; set; }
        public string Status { get; set; } = "successful";
        public long Amount { get; set; }
        public long ProcessingFee { get; set; } = 0;
        public string BankAccount { get; set; }
        public string AccountNumber { get; set; }
        public string Remarks { get; set; } = "None";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    
    }
}
