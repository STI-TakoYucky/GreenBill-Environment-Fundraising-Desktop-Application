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
    public class DonationRecordService : IDonationRecordService
    {
        private readonly IMongoCollection<DonationRecord> _collection;

        public DonationRecordService()
        {
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("GreenBill");
            _collection = database.GetCollection<DonationRecord>("DonationRecord");
        }

        public async Task Create(DonationRecord donationRecord)
        {
            await _collection.InsertOneAsync(donationRecord);
        }

        public async Task<List<DonationRecord>> GetByCampaignIdAsync(ObjectId campaignId)
        {
            try
            {
                var filter = Builders<DonationRecord>.Filter.Eq(x => x.CampaignId, campaignId);

                var sort = Builders<DonationRecord>.Sort.Descending(x => x.CreatedAt);

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
