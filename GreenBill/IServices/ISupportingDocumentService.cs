using GreenBill.MVVM.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.IServices
{
    public interface ISupportingDocumentService
    {
        Task Create(SupportingDocument supportingDocument);
        Task<List<SupportingDocument>> GetByCampaignIdAsync(string campaignId);
        Task<List<SupportingDocument>> GetByUserIdAsync(string userId);
        Task<SupportingDocument> GetByIdAsync(string documentId);
        Task UpdateStatusAsync(string documentId, string status, string comments = null);
        Task DeleteAsync(string documentId);
        Task<List<SupportingDocument>> GetAllAsync();
        Task<List<SupportingDocument>> GetByStatusAsync(string status);
        void ApproveSupportingDocument(object campaignId);
        void StageReviewSupportingDocument(object campaignId);
        void RejectDocument(ObjectId campaignId);
    }
}
