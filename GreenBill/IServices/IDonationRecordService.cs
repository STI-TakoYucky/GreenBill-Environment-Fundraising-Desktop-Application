using GreenBill.MVVM.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.IServices
{
    public interface IDonationRecordService
    {
        Task Create(DonationRecord donationRecord);
        Task<List<DonationRecord>> GetByCampaignIdAsync(ObjectId campaignId);
        Task<List<DonationRecord>> GetAllCampaignsAsync();
    }
}
