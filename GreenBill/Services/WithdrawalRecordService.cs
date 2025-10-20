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
    public class WithdrawalRecordService : IWithdrawalRecordService
    {
        private readonly IMongoCollection<WithdrawalRecord> _collection;

        public WithdrawalRecordService()
        {
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("GreenBill");
            _collection = database.GetCollection<WithdrawalRecord>("WithdrawalRecords");
        }

        public async Task Create(WithdrawalRecord withdrawalRecord)
        {
            try
            {
                await _collection.InsertOneAsync(withdrawalRecord);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating supporting document: {ex.Message}", ex);
            }
        }

        public async Task<List<WithdrawalRecord>> GetAllDonationsByIdAsync(ObjectId campaignId)
        {
            try
            {
                var filter = Builders<WithdrawalRecord>.Filter.Eq(x => x.CampaignId, campaignId);

                var sort = Builders<WithdrawalRecord>.Sort.Descending(x => x.CreatedAt);

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
