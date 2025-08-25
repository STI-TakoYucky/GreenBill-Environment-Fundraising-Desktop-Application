using GreenBill.MVVM.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.Validators
{
    public class CampaignValidator
    {
        public static Dictionary<string, string> Validate(Campaign campaign, ArrayList list = null)
        {
            Dictionary<string, string> errorList = new Dictionary<string, string>();

            bool validateAll = (list == null || list.Count == 0);

            if (validateAll || list.Contains("Title"))
                if (string.IsNullOrWhiteSpace(campaign.Title))
                    errorList.Add("Title", "This Field is required");

            if (validateAll || list.Contains("Description"))
                if (string.IsNullOrWhiteSpace(campaign.Description))
                    errorList.Add("Description", "This Field is required");

            if (validateAll || list.Contains("DonationGoal"))
                if (campaign.DonationGoal <= 0)
                    errorList.Add("DonationGoal", "The donation goal should be at least 100");

            if (validateAll || list.Contains("Country"))
                if (string.IsNullOrWhiteSpace(campaign.Country))
                    errorList.Add("Country", "This Field is required");

            if (validateAll || list.Contains("ZipCode"))
                if (string.IsNullOrWhiteSpace(campaign.ZipCode))
                    errorList.Add("ZipCode", "This Field is required");

            if (validateAll || list.Contains("Category"))
                if (string.IsNullOrWhiteSpace(campaign.Category))
                    errorList.Add("Category", "This Field is required");

            if (validateAll || list.Contains("Image"))
                if (campaign.Image == null || campaign.Image.Length == 0)
                    errorList.Add("Image", "This Field is required");

            return errorList;
        }

    }
}
