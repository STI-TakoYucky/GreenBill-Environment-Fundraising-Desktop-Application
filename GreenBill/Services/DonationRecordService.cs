using GreenBill.IServices;
using GreenBill.MVVM.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
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
    }
}
