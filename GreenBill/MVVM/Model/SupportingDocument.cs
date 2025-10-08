using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel;
using System.Net.NetworkInformation;

namespace GreenBill.MVVM.Model
{
    public class SupportingDocument : INotifyPropertyChanged {
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

        private string _status = "in review";
        public string Status {
            get => _status;
            set {
                if (_status != value) {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        } 

        public string ReviewComments { get; set; }
        public string UserId { get; set; } 

        public SupportingDocument()
        {
            Id = ObjectId.GenerateNewId().ToString();
            UploadDate = DateTime.UtcNow;
            Status = "Pending";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}