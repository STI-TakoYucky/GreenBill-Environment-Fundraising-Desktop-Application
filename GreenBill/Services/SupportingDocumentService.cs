using GreenBill.IServices;
using GreenBill.MVVM.Model;
using GreenBill.MVVM.ViewModel.Admin;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.Services
{
    public class SupportingDocumentService : ISupportingDocumentService
    {
        private readonly IMongoCollection<SupportingDocument> _collection;

        public SupportingDocumentService()
        {
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("GreenBill");
            _collection = database.GetCollection<SupportingDocument>("SupportingDocuments");
        }
        public async Task Create(SupportingDocument supportingDocument)
        {
            await _collection.InsertOneAsync(supportingDocument);
        }
    }
}
