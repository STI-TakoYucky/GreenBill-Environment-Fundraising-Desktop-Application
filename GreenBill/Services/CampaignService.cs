using GreenBill.MVVM.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenBill.IServices;
using System.Windows;

namespace GreenBill.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IMongoCollection<Campaign> _collection;
        private readonly IUserService _userService;
        private readonly ICampaignUpdateService _campaignUpdateService;
        private readonly IDonationRecordService _donationRecordService;

        public CampaignService(IUserService userService, IDonationRecordService donationRecordService, ICampaignUpdateService campaignUpdateService)
        {
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("GreenBill");
            _collection = database.GetCollection<Campaign>("Campaigns");
            _userService = userService;
            _donationRecordService = donationRecordService;
            _campaignUpdateService = campaignUpdateService;
        }

        public async Task<List<Campaign>> GetAllCampaignsAsync(CampaignIncludeOptions options = null)
        {
            var campaigns = await _collection.Find(_ => true).ToListAsync();

            // Load related data if requested
            if (options?.IncludeUser == true)
            {
                await LoadUsersForCampaigns(campaigns);
            }
            if (options?.IncludeDonationRecord == true)
            {
                await LoadDonationRecordsForCampaigns(campaigns);
            }


            return campaigns;
        }

        public async Task<List<Campaign>> GetAllCampaignsByIdAsync(ObjectId id, CampaignIncludeOptions options = null)
        {
            var campaigns = await _collection
                .Find(c => c.UserId == id)
                .ToListAsync();

            // Load related data if requested
            if (options?.IncludeUser == true)
            {
                // Since all campaigns belong to the same user, we can load it once
                var user = await _userService.GetUserByIdAsync(id.ToString());
                campaigns.ForEach(c => c.User = user);
            }
            if (options?.IncludeDonationRecord == true)
            {
                await LoadDonationRecordsForCampaigns(campaigns);
            }


            return campaigns;
        }

        public async Task<Campaign> GetCampaignByIdAsync(string id, CampaignIncludeOptions options = null)
        {
            var objectId = ObjectId.Parse(id);
            var campaign = await _collection.Find(c => c.Id == objectId).FirstOrDefaultAsync();

            if (campaign == null) return null;

            // Load related data if requested
            if (options?.IncludeUser == true)
            {
                campaign.User = await _userService.GetUserByIdAsync(campaign.UserId.ToString());
            }

            if (options?.IncludeDonationRecord == true)
            {
                campaign.DonationRecord = await _donationRecordService.GetByCampaignIdAsync(campaign.Id);
                campaign.DonationsCount = campaign.DonationRecord.Count.ToString() + " donations";
                var total = $"${(campaign.DonationRecord?.Sum(item => item.RealAmount) ?? 0):N2} USD raised";
                campaign.TotalAmountRaised = total;
                var percentage = ((campaign.DonationRecord?.Sum(item => item.RealAmount) ?? 0) / campaign.DonationGoal) * 100;
                campaign.Percentage = $"{percentage}% Funded";

            }
            if(options?.IncludeCampaignUpdate == true)
            {
                campaign.CampaignUpdate = await _campaignUpdateService.GetByCampaignIdAsync(campaign.Id);
            }

            return campaign;
        }

        public async void Create(Campaign campaign)
        {
            if (campaign.CreatedAt == default(DateTime))
            {
                campaign.CreatedAt = DateTime.UtcNow;
            }
            await _collection.InsertOneAsync(campaign);
        }

        // Helper method for batch loading users
        private async Task LoadUsersForCampaigns(List<Campaign> campaigns)
        {
            if (!campaigns.Any()) return;

            // Get unique user IDs to avoid duplicate queries
            var userIds = campaigns.Select(c => c.UserId).Distinct().ToList();
            var users = new Dictionary<ObjectId, User>();

            // Batch load users
            foreach (var userId in userIds)
            {
                var user = await _userService.GetUserByIdAsync(userId.ToString());
                if (user != null)
                {
                    users[userId] = user;
                }
            }

            // Assign users to campaigns
            foreach (var campaign in campaigns)
            {
                if (users.ContainsKey(campaign.UserId))
                {
                    campaign.User = users[campaign.UserId];
                }
            }
        }


        private async Task LoadDonationRecordsForCampaigns(List<Campaign> campaigns)
        {
            if (!campaigns.Any()) return;
            
            // Load donation records for each campaign
            foreach (var campaign in campaigns)
            {
                try
                {
                    campaign.DonationRecord = await _donationRecordService.GetByCampaignIdAsync(campaign.Id);
                    campaign.DonationsCount = campaign.DonationRecord.Count.ToString() + " donations"; 
                    var total = $"${(campaign.DonationRecord?.Sum(item => item.RealAmount) ?? 0):N2} USD raised";
                    campaign.TotalAmountRaised = total;
                    var percentage = ((campaign.DonationRecord?.Sum(item => item.RealAmount) ?? 0) / campaign.DonationGoal) * 100;
                    campaign.Percentage = $"{percentage}% Funded";
                }
                catch (Exception ex)
                {
                    // Log the exception if you have logging configured
                    // For now, initialize with empty list to prevent null reference issues
                    campaign.DonationRecord = new List<DonationRecord>();
                }
            }
        }

        public async void ApproveCampaign(ObjectId id) {
            
            //var campaign = await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
            
            var filter = Builders<Campaign>.Filter.Eq("_id", id);

            var update = Builders<Campaign>.Update
                .Set(campaign => campaign.Status, "Verified");

            var result = _collection.UpdateOne(filter, update);

            if (result != null) {
                MessageBox.Show("Approved Successfully");
            } else {
                MessageBox.Show("Campaign Not Found");
            }
        }

        public async void StageReviewCampaign(ObjectId id) {

            //var campaign = await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();

            var filter = Builders<Campaign>.Filter.Eq("_id", id);

            var update = Builders<Campaign>.Update
                .Set(campaign => campaign.Status, "in review");

            var result = _collection.UpdateOne(filter, update);

            if (result != null) {
                MessageBox.Show("Campaign back to reviewing stage.");
            } else {
                MessageBox.Show("Campaign Not Found");
            }
        }

        public async void RejectCampaign(ObjectId id) {
            var filter = Builders<Campaign>.Filter.Eq("_id", id);
            var update = Builders<Campaign>.Update.Set(campaign => campaign.Status, "Rejected");
            var result = _collection.UpdateOne(filter, update);

            if (result != null) {
                MessageBox.Show("Campaign rejected");
            } else {
                MessageBox.Show("Campaign Not Found");
            }
        }

    }
}