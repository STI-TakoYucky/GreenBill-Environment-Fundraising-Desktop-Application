using GreenBill.MVVM.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBill.IServices
{
    public interface IWithdrawalRecordService
    {
        Task Create(WithdrawalRecord withdrawalRecord);
        Task<List<WithdrawalRecord>> GetAllDonationsByIdAsync(ObjectId withdrawalId);
    }
}
