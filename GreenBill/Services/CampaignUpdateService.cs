using GreenBill.IServices;
using GreenBill.MVVM.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                Debug.WriteLine(campaignId.ToString());
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

        public async Task UpdateAsync(ObjectId updateId, CampaignUpdate campaignUpdate)
        {
            try
            {
                var filter = Builders<CampaignUpdate>.Filter.Eq(x => x.Id, updateId);
                var options = new ReplaceOptions { IsUpsert = false };
                var result = await _collection.ReplaceOneAsync(filter, campaignUpdate, options);
                if (result.MatchedCount == 0)
                {
                    throw new Exception("Document not found or could not be updated");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating document {updateId}: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(ObjectId updateId)
        {
            try
            {
                var filter = Builders<CampaignUpdate>.Filter.Eq(x => x.Id, updateId);
                var result = await _collection.DeleteOneAsync(filter);

                if (result.DeletedCount == 0)
                {
                    throw new Exception("Document not found or could not be deleted");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting document {updateId}: {ex.Message}", ex);
            }
        }

    }
}
