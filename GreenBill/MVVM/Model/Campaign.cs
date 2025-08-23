using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.MVVM.Model
{
    public class Campaign
    {
        public ObjectId Id { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; } 
        public string Category { get; set; }
        public decimal DonationGoal { get; set; }
        public byte[] Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
         public DateTime CreatedAt { get; set; }
        public Campaign()
        {
        }

        public Campaign(string country, string zipCode, string category, decimal donationGoal, byte[] image, string title, string description)
        {
            Country = country;
            ZipCode = zipCode;
            Category = category;
            DonationGoal = donationGoal;
            Image = image;
            Title = title;
            Description = description;
        }
    }
}