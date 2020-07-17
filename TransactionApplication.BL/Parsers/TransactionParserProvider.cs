using System;
using System.IO;
using System.Threading.Tasks;
using TransactionApplication.BL.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace TransactionApplication.BL.Parsers
{
    public class TransactionParserProvider : ITransactionParserProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public TransactionParserProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<ITransactionDataParser> GetParser(Stream data)
        {
            if (!data.CanSeek)
            {
                throw new ArgumentException();
            }

            var availableParsers = _serviceProvider.GetServices<ITransactionDataParser>();

            foreach (var parser in availableParsers)
            {
                data.Seek(0, SeekOrigin.Begin);

                if (await parser.CanParseData(data))
                {
                    data.Seek(0, SeekOrigin.Begin);
                    parser.SetDataSource(data);

                    return parser;
                }

                parser.Dispose();
            }

            return null;
        }
    }
}