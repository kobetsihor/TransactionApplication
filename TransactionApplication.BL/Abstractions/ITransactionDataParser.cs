using System;
using System.IO;
using System.Threading.Tasks;
using TransactionApplication.BL.Models;

namespace TransactionApplication.BL.Abstractions
{
    public interface ITransactionDataParser : IDisposable
    {
        Task<ParseResults> ParseAllFile();

        Task<bool> CanParseData(Stream data);

        void SetDataSource(Stream data);
    }
}
