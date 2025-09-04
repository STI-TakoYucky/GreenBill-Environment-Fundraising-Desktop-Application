using GreenBill.MVVM.Model;
using GreenBill.Services;
using System;
using System.Collections.ObjectModel;

namespace GreenBill.MVVM.ViewModel.Admin {

    public class Campaign {
        public int CampaignID { get; set; }
        public string Title { get; set; }
        public string Organizer { get; set; }
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
                Organizer = "John Doe",
                DateSubmitted = DateTime.Now
            });

            Campaigns.Add(new Campaign {
                CampaignID = 1,
                Title = "Charity Run",
                Organizer = "John Doe",
                DateSubmitted = DateTime.Now
            });

            Campaigns.Add(new Campaign {
                CampaignID = 1,
                Title = "Charity Run",
                Organizer = "John Doe",
                DateSubmitted = DateTime.Now
            });

            Campaigns.Add(new Campaign {
                CampaignID = 1,
                Title = "Charity Run",
                Organizer = "John Doe",
                DateSubmitted = DateTime.Now
            });

        }
    }
}
