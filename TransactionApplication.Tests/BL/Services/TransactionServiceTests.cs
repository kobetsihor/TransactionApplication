using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Moq;
using NUnit.Framework;
using TransactionApplication.BL.Abstractions;
using TransactionApplication.BL.Models;
using TransactionApplication.BL.Services;
using TransactionApplication.DAL.Entities;
using TransactionApplication.DAL.Repositories;

namespace TransactionApplication.Tests.BL.Services
{
    public class TransactionServiceTests
    {
        private Fixture _fixture;
        private ITransactionService _transactionsService;
        private Mock<ITransactionDataParser> _transactionsDataParser;
        private Mock<ITransactionParserProvider> _transactionsParserProviderMock;
        private Mock<ITransactionRepository> _transactionRepositoryMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Init()
        {
            _fixture = new Fixture();
            _mapperMock = new Mock<IMapper>();
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _transactionsParserProviderMock = new Mock<ITransactionParserProvider>();
            _transactionsDataParser = new Mock<ITransactionDataParser>();
            _transactionsService = new TransactionService(_transactionsParserProviderMock.Object,
                                                          _transactionRepositoryMock.Object,
                                                          _mapperMock.Object);
        }

        [Test]
        public async Task GetFilteredShouldReturnModels()
        {
            var status = "status";
            var currencyCode = "currencyCode";
            var entities = _fixture.CreateMany<Transaction>();
            var models = _fixture.CreateMany<TransactionModel>();

            _transactionRepositoryMock.Setup(x => x.GetFiltered(currencyCode, null, null, status))
                                                                            .ReturnsAsync(entities);

            _mapperMock.Setup(x => x.Map<IEnumerable<TransactionModel>>(entities)).Returns(models);

            var result = await _transactionsService.GetFilteredTransactions(currencyCode, null, null, status);
          
            Assert.AreEqual(models, result);
            _transactionRepositoryMock.Verify(x => x.GetFiltered(currencyCode, null, null, status), Times.Once);
            _mapperMock.Verify(x => x.Map<IEnumerable<TransactionModel>>(entities), Times.Once);
        }

        [Test]
        public async Task AddTransactionsShouldBeHandled()
        {
            const string csvString = @"“Invoice777”,”600.00”,”EUR”,”10/17/2019 02:04:59”, “Done”";
            var csvDataStream = new MemoryStream(Encoding.UTF8.GetBytes(csvString));
            const string csvDatePattern = "dd/MM/yyyy hh:mm:ss";

            var publicId = "Invoice777";
            var code = "EUR";
            var status = "D";
            var amount = 600;
            var date = DateTimeOffset.ParseExact("10/17/2019 02:04:59", csvDatePattern, CultureInfo.InvariantCulture);

            var entity = new Transaction
            {
                PublicId = publicId,
                Amount = amount,
                Date = date,
                Code = code,
                Status = status
            };

            var model = new TransactionCreateModel
            {
                PublicId = publicId,
                Amount = amount,
                Date = date,
                Code = code,
                Status = status
            };

            var entities = new List<Transaction> { entity };
            var models = new List<TransactionCreateModel> { model };
            var parseResults = new ParseResults(models);

            _transactionsParserProviderMock.Setup(x => x.GetParser(csvDataStream)).ReturnsAsync(_transactionsDataParser.Object);
            _transactionsDataParser.Setup(x => x.CanParseData(csvDataStream)).ReturnsAsync(true);
            _transactionsDataParser.Setup(x => x.ParseAllFile()).ReturnsAsync(parseResults);
            _mapperMock.Setup(x => x.Map<IEnumerable<Transaction>>(models)).Returns(entities);
            _transactionRepositoryMock.Setup(x => x.Add(entity));

            await _transactionsService.AddTransactions(csvDataStream);

            _transactionsParserProviderMock.Verify(x => x.GetParser(csvDataStream), Times.Once);
            _transactionsDataParser.Verify(x => x.CanParseData(csvDataStream), Times.Once);
            _transactionsDataParser.Verify(x => x.ParseAllFile(), Times.Once);
            _mapperMock.Verify(x => x.Map<IEnumerable<Transaction>>(models), Times.Once);
            _transactionRepositoryMock.Verify(x => x.Add(entity), Times.Once);
        }
    }
}
