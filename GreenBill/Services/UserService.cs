using GreenBill.MVVM.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GreenBill.IServices;

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
            if (user.CreatedAt == default(DateTime))
            {
                user.CreatedAt = DateTime.UtcNow;
            }

            await Collection.InsertOneAsync(user);
        }
    }
}
