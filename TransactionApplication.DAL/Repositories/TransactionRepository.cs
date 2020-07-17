using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TransactionApplication.DAL.Context;
using TransactionApplication.DAL.Entities;

namespace TransactionApplication.DAL.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ITransactionContext _context;

        public TransactionRepository(ITransactionContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAll()
        {
            return await Entities.ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetFiltered(string currencyCode,
                                                                DateTimeOffset? startDate,
                                                                DateTimeOffset? endDate,
                                                                string status)
        {
            var query = Entities;
            if (!string.IsNullOrEmpty(currencyCode))
            {
                query = query.Where(x => x.Code.Equals(currencyCode, StringComparison.InvariantCultureIgnoreCase));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.Status.Equals(status, StringComparison.InvariantCultureIgnoreCase));
            }

            if (startDate != null && endDate != null)
            {
                query = query.Where(x => x.Date >= startDate && x.Date <= endDate);
            }

            return await query.ToListAsync();
        }

        public async Task Add(Transaction entity)
        {
            await using var transaction = await _context.BeginTransaction();
            try
            {
                await _context.Transactions.AddAsync(entity);
                await _context.SaveChanges();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }

        private IQueryable<Transaction> Entities => _context.Transactions.AsNoTracking();
    }
}