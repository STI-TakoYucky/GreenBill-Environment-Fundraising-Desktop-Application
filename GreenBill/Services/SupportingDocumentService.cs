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
            try
            {
                if (supportingDocument == null)
                    throw new ArgumentNullException(nameof(supportingDocument));

                // Ensure the document has required fields
                if (string.IsNullOrEmpty(supportingDocument.Id))
                {
                    supportingDocument.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                }

                if (supportingDocument.UploadDate == default(DateTime))
                {
                    supportingDocument.UploadDate = DateTime.UtcNow;
                }

                if (string.IsNullOrEmpty(supportingDocument.Status))
                {
                    supportingDocument.Status = "Pending";
                }

                await _collection.InsertOneAsync(supportingDocument);
            }
            catch (Exception ex)
            {
                // Log the error if you have a logging system
                // For now, re-throw the exception to be handled by the calling code
                throw new Exception($"Error creating supporting document: {ex.Message}", ex);
            }
        }

        // Additional methods you can implement later
        public async Task<List<SupportingDocument>> GetByUserIdAsync(string userId)
        {
            try
            {
                var filter = Builders<SupportingDocument>.Filter.Eq(x => x.UserId, userId);
                var result = await _collection.Find(filter).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving documents for user {userId}: {ex.Message}", ex);
            }
        }

        public async Task<SupportingDocument> GetByIdAsync(string documentId)
        {
            try
            {
                var filter = Builders<SupportingDocument>.Filter.Eq(x => x.Id, documentId);
                var result = await _collection.Find(filter).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving document {documentId}: {ex.Message}", ex);
            }
        }

        public async Task UpdateStatusAsync(string documentId, string status, string comments = null)
        {
            try
            {
                var filter = Builders<SupportingDocument>.Filter.Eq(x => x.Id, documentId);
                var update = Builders<SupportingDocument>.Update
                    .Set(x => x.Status, status);

                if (!string.IsNullOrEmpty(comments))
                {
                    update = update.Set(x => x.ReviewComments, comments);
                }

                await _collection.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating document status: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(string documentId)
        {
            try
            {
                var filter = Builders<SupportingDocument>.Filter.Eq(x => x.Id, documentId);
                await _collection.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting document {documentId}: {ex.Message}", ex);
            }
        }
    }
}