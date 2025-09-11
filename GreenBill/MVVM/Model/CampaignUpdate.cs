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
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "General Update";
        public string Tags { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public List<string> TagsList =>
            string.IsNullOrWhiteSpace(Tags)
                ? new List<string>()
                : Tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                     .Select(t => t.Trim())
                     .Where(t => !string.IsNullOrEmpty(t))
                     .ToList();

        public Visibility CategoryVisibility =>
            string.IsNullOrWhiteSpace(Category) || Category == "General Update"
                ? Visibility.Collapsed
                : Visibility.Visible;

        public Visibility TagsVisibility =>
            TagsList.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
    }
}
