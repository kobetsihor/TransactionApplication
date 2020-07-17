using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TransactionApplication.BL.Abstractions;
using TransactionApplication.BL.Exceptions;
using TransactionApplication.BL.Helpers;
using TransactionApplication.BL.Models;

namespace TransactionApplication.BL.Parsers
{
    internal class XmlTransactionParser : ITransactionDataParser
    {
        private static string DatePattern => "dd/MM/yyyy hh:mm:ss";
        private StreamReader _dataSourceReader;

        public void Dispose()
        {
            _dataSourceReader?.Dispose();
        }

        public async Task<ParseResults> ParseAllFile()
        {
            var transactionsDto = await DeserializeTransactions();
            var results = new List<TransactionCreateModel>();
            if (transactionsDto != null)
            {
                results.AddRange(from dto in transactionsDto.Transactions
                                 let date = ParseDate(dto)
                                 select new TransactionCreateModel
                                 {
                                     PublicId = dto.PublicId,
                                     Amount = Convert.ToDecimal(dto.PaymentDetails.Amount),
                                     Code = dto.PaymentDetails.CurrencyCode,
                                     Date = date,
                                     Status = StatusHelper.GetUnifiedStatus(dto.Status)
                                 });
            }

            return new ParseResults(results);
        }

        private static DateTimeOffset ParseDate(TransactionItem dto)
        {
            DateTimeOffset.TryParseExact(dto.TransactionDate, DatePattern, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date);
            return date;
        }

        private async Task<XmlTransactionsDto> DeserializeTransactions()
        {
            var xmlString = await _dataSourceReader.ReadToEndAsync();
            xmlString = xmlString.Replace("”", "\"").Replace("\r\n", string.Empty);
            var serializer = new XmlSerializer(typeof(XmlTransactionsDto));
            XmlTransactionsDto transactionsDto;
            using (var reader = new StringReader(xmlString))
            {
                try
                {
                    transactionsDto = (XmlTransactionsDto)serializer.Deserialize(reader);
                }
                catch (Exception ex)
                {
                    throw new InvalidFileException(new List<string> { ex.InnerException?.Message });
                }
            }

            return transactionsDto;
        }

        public async Task<bool> CanParseData(Stream stream)
        {
            const string rootNodeName = "Transactions";
            var canParse = false;
            try
            {
                using var reader = XmlReader.Create(stream);
                if (await reader.MoveToContentAsync() == XmlNodeType.Element
                    && string.Equals(reader.Name, rootNodeName, StringComparison.InvariantCultureIgnoreCase))
                {
                    canParse = true;
                }
            }
            catch
            {
                // ignored
            }

            return canParse;
        }

        public void SetDataSource(Stream data)
        {
            if (data == null)
            {
                throw new ArgumentException();
            }

            data.Seek(0, SeekOrigin.Begin);
            _dataSourceReader = new StreamReader(data);
        }
    }
}
