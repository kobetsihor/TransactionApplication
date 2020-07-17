using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TransactionApplication.BL.Abstractions;
using TransactionApplication.BL.Helpers;
using TransactionApplication.BL.Models;

namespace TransactionApplication.BL.Parsers
{
    internal class CsvTransactionParser : ITransactionDataParser
    {
        private StreamReader _dataSourceReader;

        private const string DatePattern = "dd/MM/yyyy hh:mm:ss";
        private const string CommaInNumberPattern = @"(?<=\d),(?=\d)";

        public async Task<bool> CanParseData(Stream stream)
        {
            using var reader = new StreamReader(stream);
            var canParse = false;
            const int minItemCount = 3;
            try
            {
                var firstLine = await reader.ReadLineAsync();

                if (firstLine != null)
                {
                    firstLine = Regex.Replace(firstLine, CommaInNumberPattern, "");
                    var items = firstLine.Split(',').Select(h => h.Trim()).ToList();
                    if (items.Count > minItemCount)
                    {
                        canParse = true;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return canParse;
        }

        public void SetDataSource(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentException();
            }

            stream.Seek(0, SeekOrigin.Begin);
            _dataSourceReader = new StreamReader(stream);
        }

        public async Task<ParseResults> ParseAllFile()
        {
            var importedItems = new List<TransactionCreateModel>();
            var item = await ParseNextRowAsync();
            while (item != null)
            {
                importedItems.Add(item);
                item = await ParseNextRowAsync();
            }

            return new ParseResults(importedItems);
        }

        public void Dispose()
        {
            _dataSourceReader?.Dispose();
        }

        private async Task<TransactionCreateModel> ParseNextRowAsync()
        {
            if (_dataSourceReader == null)
            {
                throw new ArgumentException();
            }

            if (_dataSourceReader.EndOfStream)
            {
                return null;
            }

            string row;
            var tryCount = 0;
            do
            {
                if (tryCount > 3 || _dataSourceReader.EndOfStream)
                {
                    return null;
                }

                tryCount++;
                row = await _dataSourceReader.ReadLineAsync();
                row = Regex.Replace(row, CommaInNumberPattern, "");
            } while (string.IsNullOrWhiteSpace(row));

            var fields = GetFieldsInRowWithPosition(row);
            return GenerateTransactionModel(fields);
        }

        private static TransactionCreateModel GenerateTransactionModel(IReadOnlyDictionary<int, string> fields)
        {
            var transaction = new TransactionCreateModel();
            transaction.PublicId = fields[0];
            var isDecimal = decimal.TryParse(fields[1], out var amount);
            transaction.Amount = isDecimal ? amount : (decimal?)null;
            transaction.Code = fields[2];
            var isCorrectDate = DateTimeOffset.TryParseExact(fields[3], DatePattern,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);
            transaction.Date = isCorrectDate ? date : (DateTimeOffset?)null;
            transaction.Status = StatusHelper.GetUnifiedStatus(fields[4]);
            return transaction;
        }

        private static Dictionary<int, string> GetFieldsInRowWithPosition(string line)
        {
            //use substring for deleting quotes or reverse quotes
            return line.Split(',')
                       .Select(v => v.Trim())
                       .Select((v, i) => new
                         {
                           Key = i, 
                           Value = v
                         })
                       .ToDictionary(o => o.Key, o => o.Value.Substring(1, o.Value.Length - 2));
        }
    }
}
