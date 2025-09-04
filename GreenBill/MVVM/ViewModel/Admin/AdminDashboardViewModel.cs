using System;
using System.Collections.ObjectModel;

namespace GreenBill.MVVM.ViewModel
{
    public class Campaign
    {
        public string CampaignID { get; set; }
        public string Title { get; set; }
        public string Organizer { get; set; }
        public string DateSubmitted { get; set; }
    }

    public class AdminDashboardViewModel
    {

        // ✅ Add campaigns here
        public ObservableCollection<Campaign> Campaigns { get; set; }

        public AdminDashboardViewModel()
        {
            // Dummy campaign data
            Campaigns = new ObservableCollection<Campaign>
            {
                new Campaign { CampaignID = "C001", Title = "Tree Planting", Organizer = "GreenOrg", DateSubmitted = "2025-08-20" },
                new Campaign { CampaignID = "C002", Title = "Beach Cleanup", Organizer = "EcoWave", DateSubmitted = "2025-08-21" },
                new Campaign { CampaignID = "C003", Title = "Food Drive", Organizer = "HelpHand", DateSubmitted = "2025-08-22" }
            };
        }
    }
}
