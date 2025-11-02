using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GreenBill.MVVM.Model {
    public class Campaign : INotifyPropertyChanged {
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }

        private string _country;
        public string Country {
            get => _country;
            set { if (_country != value) { _country = value; OnPropertyChanged(); } }
        }

        private string _zipCode;
        public string ZipCode {
            get => _zipCode;
            set { if (_zipCode != value) { _zipCode = value; OnPropertyChanged(); } }
        }

        private string _category;
        public string Category {
            get => _category;
            set { if (_category != value) { _category = value; OnPropertyChanged(); } }
        }

        private decimal _donationGoal;
        public decimal DonationGoal {
            get => _donationGoal;
            set { if (_donationGoal != value) { _donationGoal = value; OnPropertyChanged(); } }
        }

        private decimal _donationRaised;
        public decimal DonationRaised {
            get => _donationRaised;
            set { if (_donationRaised != value) { _donationRaised = value; OnPropertyChanged(); } }
        }

        private byte[] _image;
        public byte[] Image {
            get => _image;
            set { if (_image != value) { _image = value; OnPropertyChanged(); } }
        }

        private string _title;
        public string Title {
            get => _title;
            set { if (_title != value) { _title = value; OnPropertyChanged(); } }
        }

        private string _description;
        public string Description {
            get => _description;
            set { if (_description != value) { _description = value; OnPropertyChanged(); } }
        }

        private DateTime _createdAt;
        public DateTime CreatedAt {
            get => _createdAt;
            set { if (_createdAt != value) { _createdAt = value; OnPropertyChanged(); } }
        }

        // IMPORTANT: Status must notify changes for bindings in DataGrid to update.
        private string _status = "in review";
        public string Status {
            get => _status;
            set {
                if (_status != value) {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [BsonIgnore]
        public User User { get; set; }

        [BsonIgnore]
        public List<DonationRecord> DonationRecord { get; set; }

        [BsonIgnore]
        public List<CampaignUpdate> CampaignUpdate { get; set; }

        [BsonIgnore]
        public List<WithdrawalRecord> WithdrawalRecord { get; set; }

        [BsonIgnore]
        public string TotalAmountRaised { get; set; }

        [BsonIgnore]
        public string DonationsCount { get; set; }

        [BsonIgnore]
        public string Percentage { get; set; }

        [BsonIgnore]
        public string DaySpan { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}