using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.MVVM.Model
{
    public class SupportingDocument
    {
        public ObjectId Id { get; set; }
        public ObjectId CampaignId { get; set; }
        public ObjectId ReviewerId { get; set; }
        public byte[] Document { get; set; }
        public string Status { get; set; } = "in review";
        public string Remarks { get; set; }
        public string StatusMessage { get; set; }
        public DateTime ReviewDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
