using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.MVVM.Model
{
    public class CampaignIncludeOptions
    {
        public bool IncludeUser { get; set; }
        public bool IncludeSupportingDocument { get; set; }
        public bool IncludeDonationRecord { get; set; }
        public bool IncludeCampaignUpdate { get; set; }
    }
}
