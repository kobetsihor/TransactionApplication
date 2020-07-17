using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TransactionApplication.DAL.Entities;

namespace TransactionApplication.DAL.Context
{
    public class TransactionContext : DbContext, ITransactionContext
    {
        public TransactionContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public virtual DbSet<Transaction> Transactions { get; set; }

        public async Task<IDbContextTransaction> BeginTransaction()
        {
            return await Database.BeginTransactionAsync();
        }

        public new async Task SaveChanges()
        {
            await base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                 .HasData(new Transaction { Id = Guid.NewGuid(), Status = "D", Amount = 600, Code = "EUR", Date = DateTimeOffset.Now },
                          new Transaction { Id = Guid.NewGuid(), Status = "R", Amount = 300, Code = "USD", Date = DateTimeOffset.Now },
                          new Transaction { Id = Guid.NewGuid(), Status = "A", Amount = 400, Code = "EUR", Date = DateTimeOffset.Now });
        }
    }
}