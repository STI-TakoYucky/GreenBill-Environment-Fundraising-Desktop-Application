using GreenBill.MVVM.Model;
using GreenBill.Services;
using System;
using System.Collections.ObjectModel;

namespace GreenBill.MVVM.ViewModel.Admin {

    public class Campaign {
        public int CampaignID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TargetDonation { get; set; }
        public string AccumulatedDonation { get; set; }
        public string Status { get; set; }
        public bool Verified { get; set; }
        public DateTime DateSubmitted { get; set; }
    }
    public class AdminDashboardViewModel : Core.ViewModel, INavigationAware {

        public ObservableCollection<Campaign> Campaigns { get; set; }

        public bool ShowNavigation => true;

        public AdminDashboardViewModel()
        {
            Campaigns = new ObservableCollection<Campaign>();

            Campaigns.Add(new Campaign {
                CampaignID = 1,
                Title = "Charity Run",
                Description = "A charity run for climate change",
                TargetDonation = "1000PHP",
                AccumulatedDonation = "200PHP",
                Status = "Ongoing",
                Verified = true,
                DateSubmitted = DateTime.Now
            });

        }
    }
}
