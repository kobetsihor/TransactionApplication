using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TransactionApplication.DAL.Entities;

namespace TransactionApplication.DAL.Context
{
    public interface ITransactionContext
    {
        DbSet<Transaction> Transactions { get; set; }
        Task<IDbContextTransaction> BeginTransaction();
        Task SaveChanges();
    }
}