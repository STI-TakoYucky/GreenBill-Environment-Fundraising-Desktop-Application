using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System;
using System.Collections.ObjectModel;
using LiveChartsCore.Kernel.Sketches;

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
        public ISeries[] Series { get; set; } = [
          new LineSeries<DateTimePoint>
        {
            Values = [
                new() { DateTime = new(2021, 1, 1), Value = 3 },
                new() { DateTime = new(2021, 1, 2), Value = 6 },
                new() { DateTime = new(2021, 1, 3), Value = 5 },
                new() { DateTime = new(2021, 1, 4), Value = 3 },
                new() { DateTime = new(2021, 1, 5), Value = 5 },
                new() { DateTime = new(2021, 1, 6), Value = 8 },
                new() { DateTime = new(2021, 1, 7), Value = 6 }
            ]
        }
      ];

        public ICartesianAxis[] XAxes { get; set; } = [
            new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("MMMM dd"))
        ];

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
