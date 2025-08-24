using GreenBill.MVVM.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _collection;
        public UserService() 
        {
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("GreenBill");
            _collection = database.GetCollection<User>("Users");
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
            
        public async Task<User> GetUserByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _collection.Find(c => c.Id == objectId).FirstOrDefaultAsync();
        }

        public async void Create(User user)
        {
            if (user.CreatedAt == default(DateTime))
            {
                user.CreatedAt = DateTime.UtcNow;
            }


            await _collection.InsertOneAsync(user);

        }
    }
}
