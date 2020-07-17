using System;

namespace TransactionApplication.BL.Models
{
    public class TransactionModel
    {
        public TransactionModel(string publicId,
                                decimal amount,
                                string code,
                                DateTimeOffset date,
                                string status)
        {
            PublicId = publicId;
            Amount = amount;
            Code = code;
            Date = date;
            Status = status;
        }

        public string PublicId { get; }
        public decimal Amount { get; }
        public string Code { get; }
        public DateTimeOffset Date { get; }
        public string Status { get; }
    }
}