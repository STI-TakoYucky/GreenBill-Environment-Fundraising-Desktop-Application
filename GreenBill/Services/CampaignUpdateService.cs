using GreenBill.IServices;
using GreenBill.MVVM.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.Services
{
    public class CampaignUpdateService : ICampaignUpdateService
    {
        private readonly IMongoCollection<CampaignUpdate> _collection;

        public CampaignUpdateService()
        {
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("GreenBill");
            _collection = database.GetCollection<CampaignUpdate>("CampaignUpdates");
        }

        public async Task Create(CampaignUpdate campaignUpdate)
        {
            try
            {
                await _collection.InsertOneAsync(campaignUpdate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating supporting document: {ex.Message}", ex);
            }
        }

        public async Task<List<CampaignUpdate>> GetByCampaignIdAsync(ObjectId campaignId)
        {
            try
            {
                var filter = Builders<CampaignUpdate>.Filter.Eq(x => x.CampaignId, campaignId);

                var sort = Builders<CampaignUpdate>.Sort.Descending(x => x.CreatedAt);

                var result = await _collection.Find(filter).Sort(sort).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving documents for campaign {campaignId}: {ex.Message}", ex);
            }
        }


    }
}
