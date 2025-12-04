using GreenBill.IServices;
using GreenBill.MVVM.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreenBill.Services
{
    public class UserService : IUserService
    {
        public IMongoCollection<User> Collection { get; }

        public UserService()
        {
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("GreenBill");
            Collection = database.GetCollection<User>("Users");
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await Collection.Find(_ => true).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await Collection.Find(c => c.Id == objectId).FirstOrDefaultAsync();
        }

        public async Task Create(User user)
        {
            await Collection.InsertOneAsync(user);
           
        }

        public async Task<bool> UpdateUserAsync(ObjectId id, User updatedUser)
        {
            updatedUser.Id = id;

            var result = await Collection.ReplaceOneAsync(
                filter: u => u.Id == id,
                replacement: updatedUser
            );

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteUserAsync(ObjectId id) {
            var result = await Collection.DeleteOneAsync(u => u.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
