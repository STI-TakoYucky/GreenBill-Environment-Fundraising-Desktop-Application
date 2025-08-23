using GreenBill.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.Services
{
    public interface ICampaignService
    {
        Task<List<Campaign>> GetAllCampaignsAsync();
        Task<Campaign> GetCampaignByIdAsync(string id);
    }
}
