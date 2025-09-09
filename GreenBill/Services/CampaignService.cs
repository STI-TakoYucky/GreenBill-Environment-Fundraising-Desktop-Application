using GreenBill.MVVM.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenBill.IServices;

namespace GreenBill.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IMongoCollection<Campaign> _collection;
        private readonly IUserService _userService;

        public CampaignService(IUserService userService)
        {
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("GreenBill");
            _collection = database.GetCollection<Campaign>("Campaigns");
            _userService = userService;
        }

        public async Task<List<Campaign>> GetAllCampaignsAsync(CampaignIncludeOptions options = null)
        {
            var campaigns = await _collection.Find(_ => true).ToListAsync();

            // Load related data if requested
            if (options?.IncludeUser == true)
            {
                await LoadUsersForCampaigns(campaigns);
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
    }
}