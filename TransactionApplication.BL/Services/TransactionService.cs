using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using TransactionApplication.BL.Abstractions;
using TransactionApplication.BL.Exceptions;
using TransactionApplication.BL.Models;
using TransactionApplication.BL.Validators;
using TransactionApplication.DAL.Entities;
using TransactionApplication.DAL.Repositories;

namespace TransactionApplication.BL.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionParserProvider _transactionParserProvider;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly ParseResultsValidator _validator = new ParseResultsValidator();

        public TransactionService(ITransactionParserProvider transactionParserProvider,
                                  ITransactionRepository transactionRepository,
                                  IMapper mapper)
        {
            _transactionParserProvider = transactionParserProvider;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TransactionModel>> GetAllTransactions()
        {
            var transactions = await _transactionRepository.GetAll();
            return _mapper.Map<IEnumerable<TransactionModel>>(transactions);
        }

        public async Task<IEnumerable<TransactionModel>> GetFilteredTransactions(string currencyCode,
                                                                                 string startDate,
                                                                                 string endDate,
                                                                                 string status)
        {
            var isValidDateRange = ValidateAndParseDateFilters(startDate, endDate, out var startDateFilter, out var endDateFilter);
            var items = await _transactionRepository.GetFiltered(currencyCode, isValidDateRange ? startDateFilter
                                                                                                                          : (DateTimeOffset?)null, isValidDateRange ? endDateFilter : (DateTimeOffset?)null, status);
            return _mapper.Map<IEnumerable<TransactionModel>>(items);
        }

        public async Task AddTransactions(Stream data)
        {
            using var parser = await _transactionParserProvider.GetParser(data);
            if (parser == null)
            {
                throw new InvalidFileException(new List<string> { "Unknown data format" });
            }

            var importedItems = await parser.ParseAllFile();
            await _validator.ValidateAndThrowAsync(importedItems);
            var transactions = _mapper.Map<IEnumerable<Transaction>>(importedItems.Transactions);

            foreach (var transaction in transactions)
            {
                await _transactionRepository.Add(transaction);
            }
        }

        private bool ValidateAndParseDateFilters(string startDate,
                                                 string endDate,
                                                 out DateTimeOffset startDateFilter,
                                                 out DateTimeOffset endDateFilter)
        {
            const string appliedDateFormat = "dd/MM/yyyy hh:mm:ss";
            var isValidDateRange = false;
            startDateFilter = new DateTimeOffset();
            endDateFilter = new DateTimeOffset();
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                var hasStartDate = DateTimeOffset.TryParseExact(startDate,
                                                                    appliedDateFormat,
                                                                    CultureInfo.InvariantCulture,
                                                                    DateTimeStyles.None, out startDateFilter);

                var hasEndDate = DateTimeOffset.TryParseExact(endDate,
                                                                appliedDateFormat,
                                                                  CultureInfo.InvariantCulture,
                                                                  DateTimeStyles.None,
                                                                  out endDateFilter);

                if (hasEndDate && hasStartDate)
                {
                    isValidDateRange = true;
                }
            }

            return isValidDateRange;
        }
    }
}
