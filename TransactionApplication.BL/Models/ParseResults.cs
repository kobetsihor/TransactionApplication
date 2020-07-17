using System.Collections.Generic;

namespace TransactionApplication.BL.Models
{
    public class ParseResults
    {
        public ParseResults(IEnumerable<TransactionCreateModel> transactions)
        {
            Transactions = transactions;
        }

        public IEnumerable<TransactionCreateModel> Transactions { get; }
    }
}