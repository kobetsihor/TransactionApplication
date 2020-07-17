using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransactionApplication.DAL.Entities;

namespace TransactionApplication.DAL.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAll();
        Task Add(Transaction entity);
        Task<IEnumerable<Transaction>> GetFiltered(string currencyCode,
                                                   DateTimeOffset? startDate,
                                                   DateTimeOffset? endDate,
                                                   string status);
    }
}