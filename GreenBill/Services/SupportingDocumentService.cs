using GreenBill.IServices;
using GreenBill.MVVM.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        // Get documents by campaign ID
        public async Task<List<SupportingDocument>> GetByCampaignIdAsync(string campaignId)
        {
            try
            {
                ObjectId campaignObjectId = ObjectId.Parse(campaignId);
                var filter = Builders<SupportingDocument>.Filter.Eq(x => x.CampaignId, campaignObjectId);

                // Sort by upload date descending (newest first)
                var sort = Builders<SupportingDocument>.Sort.Descending(x => x.UploadDate);

                var result = await _collection.Find(filter).Sort(sort).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving documents for campaign {campaignId}: {ex.Message}", ex);
            }
        }

        // Additional methods you can implement later
        public async Task<List<SupportingDocument>> GetByUserIdAsync(string userId)
        {
            try
            {
                var filter = Builders<SupportingDocument>.Filter.Eq(x => x.UserId, userId);
                var sort = Builders<SupportingDocument>.Sort.Descending(x => x.UploadDate);
                var result = await _collection.Find(filter).Sort(sort).ToListAsync();
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
                var result = await _collection.DeleteOneAsync(filter);

                if (result.DeletedCount == 0)
                {
                    throw new Exception("Document not found or could not be deleted");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting document {documentId}: {ex.Message}", ex);
            }
        }

        public async Task<List<SupportingDocument>> GetAllAsync()
        {
            try
            {
                var sort = Builders<SupportingDocument>.Sort.Descending(x => x.UploadDate);
                var result = await _collection.Find(FilterDefinition<SupportingDocument>.Empty)
                                             .Sort(sort)
                                             .ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all documents: {ex.Message}", ex);
            }
        }

        public async Task<List<SupportingDocument>> GetByStatusAsync(string status)
        {
            try
            {
                var filter = Builders<SupportingDocument>.Filter.Eq(x => x.Status, status);
                var sort = Builders<SupportingDocument>.Sort.Descending(x => x.UploadDate);
                var result = await _collection.Find(filter).Sort(sort).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving documents with status {status}: {ex.Message}", ex);
            }
        }

        public async void ApproveSupportingDocument(object id) {

            //var campaign = await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();

            var filter = Builders<SupportingDocument>.Filter.Eq("_id", id);

            var update = Builders<SupportingDocument>.Update
                .Set(document => document.Status, "Verified");

            var result = _collection.UpdateOne(filter, update);

            if (result != null) {
                MessageBox.Show("Document verified successfully");
            } else {
                MessageBox.Show("NOT FOUND");
            }
        }
        public async void StageReviewSupportingDocument(object id) {

            //var campaign = await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();

            var filter = Builders<SupportingDocument>.Filter.Eq("_id", id);

            var update = Builders<SupportingDocument>.Update
                .Set(document => document.Status, "in review");

            var result = _collection.UpdateOne(filter, update);

            if (result != null) {
                MessageBox.Show("Document back to reviewing stage");
            } else {
                MessageBox.Show("NOT FOUND");
            }
        }

        public async void RejectDocument(ObjectId id) {
            var filter = Builders<SupportingDocument>.Filter.Eq("_id", id);
            var update = Builders<SupportingDocument>.Update.Set(document => document.Status, "Rejected");
            var result = _collection.UpdateOne(filter, update);

            if (result != null) {
                MessageBox.Show("Document rejected");
            } else {
                MessageBox.Show("Document Not Found");
            }
        }

        public async void UpdateComments(ObjectId id, string comment) {
            var filter = Builders<SupportingDocument>.Filter.Eq("_id", id);
            var update = Builders<SupportingDocument>.Update.Set(document => document.ReviewComments, comment);
            var result = _collection.UpdateOne(filter, update);

            if (result == null) {
                MessageBox.Show("Failed to add comments");
            }
        }

    }
}