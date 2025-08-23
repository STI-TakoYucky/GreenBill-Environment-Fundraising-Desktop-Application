using GreenBill.MVVM.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IMongoCollection<Campaign> _collection;

        public CampaignService()
        {
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("GreenBill");
            _collection = database.GetCollection<Campaign>("Campaigns");
        }

        public async Task<List<Campaign>> GetAllCampaignsAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Campaign> GetCampaignByIdAsync(string id)
        {
            return await _collection.Find(c => c.Id.Equals(id)).FirstOrDefaultAsync();
        }
    }
}
