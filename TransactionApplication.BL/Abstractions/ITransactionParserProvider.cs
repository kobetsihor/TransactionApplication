using System.IO;
using System.Threading.Tasks;

namespace TransactionApplication.BL.Abstractions
{
    public interface ITransactionParserProvider
    {
        Task<ITransactionDataParser> GetParser(Stream data);
    }
}
