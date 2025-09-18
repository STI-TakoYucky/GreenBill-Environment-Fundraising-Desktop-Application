using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GreenBill.MVVM.Model
{
    public class SupportingDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public ObjectId CampaignId { get; set; }
        public string FileName { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public byte[] FileData { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public string Status { get; set; } 
        public string ReviewComments { get; set; }
        public string UserId { get; set; } 

        public SupportingDocument()
        {
            Id = ObjectId.GenerateNewId().ToString();
            UploadDate = DateTime.UtcNow;
            Status = "Pending";
        }
    }
}