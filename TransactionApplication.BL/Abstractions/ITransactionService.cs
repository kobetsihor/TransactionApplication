using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TransactionApplication.BL.Models;

namespace TransactionApplication.BL.Abstractions
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionModel>> GetAllTransactions();
        Task AddTransactions(Stream data);
        Task<IEnumerable<TransactionModel>> GetFilteredTransactions(string currencyCode,
                                                                    string startDate,
                                                                    string endDate,
                                                                    string status);
    }
}