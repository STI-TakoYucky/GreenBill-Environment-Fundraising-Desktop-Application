using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Documents;

namespace GreenBill.MVVM.Model
{
    public class Campaign : INotifyPropertyChanged {
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