using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GreenBill.MVVM.Model
{
    public class CampaignUpdate
    {
        public ObjectId Id { get; set; }
        public ObjectId CampaignId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "General Update";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Visibility CategoryVisibility =>
            string.IsNullOrWhiteSpace(Category) || Category == "General Update"
                ? Visibility.Collapsed
                : Visibility.Visible;

    }
}
