using MFGApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.data;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFGApi.Controllers
{
    [EnableCors("AllowAll")]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly BankAppDataContext _context;
        public AccountsController(BankAppDataContext context)
        {
            _context = context;
        }

        [Route("{id}")]
        [HttpGet]
        public ActionResult<AccountsApiViewModel> Account(int id, int offset = 0, int limit = 50)
        {
           var query = _context.Transactions.Where(c => c.AccountId == id).Skip(offset).Take(limit).OrderByDescending(c=>c.Date);
            var viewModel = new AccountsApiViewModel();
            viewModel.Transactions = query.Skip(0).Take(20).Select(dbTransact => new AccountsApiViewModel.TransactionsViewModel
            {
                AccountId = dbTransact.AccountId,
                TransactionId = dbTransact.TransactionId,
                Account = dbTransact.Account,
                Amount = dbTransact.Amount,
                Balance = dbTransact.Balance,
                Date = dbTransact.Date,
                Bank = dbTransact.Bank,
                Operation = dbTransact.Operation,
                Symbol = dbTransact.Symbol,
                Type = dbTransact.Type



            }).ToList();


            if (viewModel == null)
            {
                return NotFound();
            }
            return Ok(viewModel);
        }
    }
}
