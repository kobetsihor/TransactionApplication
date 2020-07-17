using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionApplication.API.Abstractions;
using TransactionApplication.API.Models;
using TransactionApplication.BL.Abstractions;
using TransactionApplication.BL.Exceptions;

namespace TransactionApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase, ITransactionsController
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            return Ok(await _transactionService.GetAllTransactions());
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFilteredTransactions(TransactionFilterModel filters)
        {
            return Ok(await _transactionService.GetFilteredTransactions(filters.CurrencyCode,
                                                                        filters.StartDate,
                                                                        filters.EndDate,
                                                                        filters.Status));
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest();
            }

            ValidateFileFormat(file);
            await _transactionService.AddTransactions(file.OpenReadStream());
            return Ok();
        }

        private void ValidateFileFormat(IFormFile file)
        {
            var extensions = new List<string> { ".csv", ".xml" };
            const int appropriateFileSize = 1024;

            if (extensions.Contains(Path.GetExtension(file.FileName)))
            {
                return;
            }

            var errors = new List<string> { "UnknownFormat" };
            if (file.Length > appropriateFileSize)
            {
                errors.Add("File size more than 1MB");
            }

            throw new InvalidFileException(errors);
        }
    }
}