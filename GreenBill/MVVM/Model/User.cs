using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.MVVM.Model
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string StripeAccountId { get; set; }
        public byte[] Profile { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string VerificationStatus { get; set; } = "pending";
        public bool CanReceiveFunds { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
