using MichaFinancialGroup.Models;
using MichaFinancialGroup.Services;
using MichaFinancialGroup.ViewModels;
using MichaFinancialGroup.ViewModels.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace MichaFinancialGroup.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly BankAppDataContext _dbContext;
        public TransactionsController(ITransactionsRepository transactionsRepository, BankAppDataContext dbContext)
        {
            _transactionsRepository = transactionsRepository;
            _dbContext = dbContext;
        }
        public IActionResult Index(string sortField, string sortOrder, int page = 1)
        {
            var viewModel = new TransactionIndexViewModel();

            var query = _transactionsRepository.GetAll();

            int totalRowCount = query.Count();


            if (string.IsNullOrEmpty(sortField))
                sortField = "Date";
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "desc";

            if (sortField == "Date")
            {
                if (sortOrder == "asc")
                    query = query.OrderBy(y => y.Date);
                else
                    query = query.OrderByDescending(y => y.Date);
            }

            int pageSize = 10;

            var pageCount = (double)totalRowCount / pageSize;
            viewModel.TotalPages = (int)Math.Ceiling(pageCount);


            //Skip - hoppa över så många
            //Take - sen ta så många

            int howManyRecordsToSkip = (page - 1) * pageSize;  // Sida 1 ->  0

            query = query.Skip(howManyRecordsToSkip).Take(pageSize);

            viewModel.Transactions = query.Select(dbTransact => new TransactionIndexViewModel.TransactionsViewModel
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


            viewModel.SortOrder = sortOrder;
            viewModel.SortField = sortField;
            viewModel.Page = page;
            viewModel.OppositeSortOrder = sortOrder == "asc" ? "desc" : "asc";

            return View(viewModel);
        }

        public IActionResult New(int id)
        {

            var viewModel = new TransactionNewViewModel();
            viewModel.Operations = GetOperationListItems();
            viewModel.Type = GetTypeListItems();
            viewModel.AccountId = id;
            var acc = _dbContext.Accounts.Where(a => a.AccountId == id).FirstOrDefault();
            viewModel.CurrentBalance = acc.Balance; 

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult New(TransactionNewViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
    
                var receiver = _dbContext.Accounts.Where(a => a.AccountId == viewModel.ToAccountId).FirstOrDefault();
                var sender = _dbContext.Accounts.Where(a => a.AccountId == viewModel.AccountId).FirstOrDefault();
                receiver.Balance += viewModel.Amount;
                sender.Balance -= viewModel.Amount;

                var dbTransact = new Transactions();
                dbTransact.Balance = viewModel.CurrentBalance - viewModel.Amount;
                dbTransact.Bank = viewModel.Bank;
                dbTransact.Date = DateTime.Now;
                dbTransact.Amount = viewModel.Amount;
                dbTransact.AccountId = viewModel.AccountId;
                dbTransact.Account = viewModel.Account;
                dbTransact.Symbol = viewModel.Symbol;
                dbTransact.Type = viewModel.selectedType;
                dbTransact.Operation = viewModel.selectedOperation;
                
                _dbContext.Transactions.Add(dbTransact);
                
                
                _dbContext.SaveChanges();
                return RedirectToAction("New", new { id = viewModel.AccountId, CurrentBalance = dbTransact.Balance });
            }

            return View(viewModel);
        }

        private List<SelectListItem> GetOperationListItems()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "0", Text = "Choose Operation" });
            list.Add(new SelectListItem { Value = "Credit In Cash", Text = "Credit In Cash" });
            list.Add(new SelectListItem { Value = "Collection From Another Bank", Text = "Collection From Another Bank" });
            list.Add(new SelectListItem { Value = "Credit", Text = "Credit" });
            list.Add(new SelectListItem { Value = "Remittance to Another Bank", Text = "Remittance to Another Bank" });
            list.Add(new SelectListItem { Value = "Withdrawal in Cash", Text = "Withdrawal in Cash" });

          
            return list;
        } 
        
        private List<SelectListItem> GetTypeListItems()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "0", Text = "Choose Transaction Type" });
            list.Add(new SelectListItem { Value = "Debit", Text = "Debit" });
            list.Add(new SelectListItem { Value = "Credit", Text = "Credit" });

          
            return list;
        }
    }
}
