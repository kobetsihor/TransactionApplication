using AutoMapper;
using TransactionApplication.BL.Models;
using TransactionApplication.DAL.Entities;

namespace TransactionApplication.BL.Configuration
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Transaction, TransactionModel>();
            CreateMap<TransactionCreateModel, Transaction> ();
        }
    }
}