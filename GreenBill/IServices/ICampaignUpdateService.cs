using GreenBill.MVVM.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.IServices
{
    public interface ICampaignUpdateService
    {
        Task Create(CampaignUpdate campaignUpdate);
        Task<List<CampaignUpdate>> GetByCampaignIdAsync(ObjectId campaignId);
        Task DeleteAsync(ObjectId updateId);

        Task UpdateAsync(ObjectId updateId, CampaignUpdate campaignUpdate);
    }
}
