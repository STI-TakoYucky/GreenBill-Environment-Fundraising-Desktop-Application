using GreenBill.MVVM.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.IServices
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(string id);
        Task Create(User user); 
        IMongoCollection<User> Collection { get; }
        Task<bool> UpdateUserAsync(ObjectId id, User updatedUser);
        Task<bool> DeleteUserAsync(ObjectId id);
    }
}
