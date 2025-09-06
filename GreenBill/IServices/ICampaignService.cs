using GreenBill.MVVM.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.IServices
{
    public interface ICampaignService
    {
        Task<List<Campaign>> GetAllCampaignsAsync();
        Task<List<Campaign>> GetAllCampaignsByIdAsync(ObjectId id);
        Task<Campaign> GetCampaignByIdAsync(string id);
        void Create(Campaign user);
    }
}
