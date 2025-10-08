using GreenBill.MVVM.Model;
using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.IServices
{
    public interface ICampaignService
    {
        Task<List<Campaign>> GetAllCampaignsAsync(CampaignIncludeOptions options = null);
        Task<List<Campaign>> GetAllCampaignsByIdAsync(ObjectId id, CampaignIncludeOptions options = null);
        Task<Campaign> GetCampaignByIdAsync(string id, CampaignIncludeOptions options = null);
        void Create(Campaign campaign);
        void ApproveCampaign(ObjectId campaignId);
        void StageReviewCampaign(ObjectId campaignId);
        void RejectCampaign(ObjectId id);
    }
}