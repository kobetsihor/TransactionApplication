using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TransactionApplication.API.Abstractions;
using TransactionApplication.API.Controllers;
using TransactionApplication.BL.Abstractions;
using TransactionApplication.BL.Models;

namespace TransactionApplication.Tests.API.Controllers
{
    public class TransactionsControllerTests
    {
        private ITransactionsController _transactionsController;
        private Mock<ITransactionService> _transactionServiceMock;
        private Fixture _fixture;

        [SetUp]
        public void Init()
        {
            _fixture = new Fixture();
            _transactionServiceMock = new Mock<ITransactionService>();
            _transactionsController = new TransactionsController(_transactionServiceMock.Object);
        }

        [Test]
        public async Task GetAllTransactionsShouldReturnSuccessResult()
        {
            var transactions = _fixture.CreateMany<TransactionModel>();

            _transactionServiceMock.Setup(x => x.GetAllTransactions()).ReturnsAsync(transactions);

            var response = await _transactionsController.GetAllTransactions() as OkObjectResult;
            var data = response?.Value as IEnumerable<TransactionModel>;

            Assert.IsNotNull(response);
            Assert.AreEqual(transactions, data);
            _transactionServiceMock.Verify(x => x.GetAllTransactions(), Times.Once);
        }
    }
}