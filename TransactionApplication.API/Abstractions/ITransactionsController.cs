using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionApplication.API.Models;

namespace TransactionApplication.API.Abstractions
{
    public interface ITransactionsController
    {
        Task<IActionResult> GetAllTransactions();
        Task<IActionResult> GetFilteredTransactions(TransactionFilterModel filters);
        Task<IActionResult> Upload(IFormFile file);
    }
}
