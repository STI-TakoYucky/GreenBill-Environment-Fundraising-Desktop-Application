using System;

namespace GreenBill.MVVM.View
{
    public class CampaignData
    {
        public string CampaignName { get; set; }
        public string AmountRaised { get; set; }
        public string Goal { get; set; }
        public string Progress { get; set; }
        public string Donations { get; set; }
        public string AvgDonation { get; set; }
        public string Status { get; set; }

        public CampaignData() { }

        public CampaignData(string campaignName, string amountRaised, string goal, string progress, string donations, string avgDonation, string status)
        {
            CampaignName = campaignName;
            AmountRaised = amountRaised;
            Goal = goal;
            Progress = progress;
            Donations = donations;
            AvgDonation = avgDonation;
            Status = status;
        }
    }
}