using MichaFinancialGroup.Services;
using MichaFinancialGroup.ViewModels;
using MichaFinancialGroup.ViewModels.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MichaFinancialGroup.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IAccountsRepository _accountsRepository;
        public TransactionsController(ITransactionsRepository transactionsRepository, IAccountsRepository accountsRepository)
        {
            _transactionsRepository = transactionsRepository;
            _accountsRepository = accountsRepository;
        }
        [Authorize(Roles = "Admin, Cashier")]
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

        [Authorize(Roles = "Admin, Cashier")]
        public IActionResult New(int id)
        {
            var viewModel = new TransactionNewViewModel();
            viewModel.Operations = GetOperationListItems();
            viewModel.AccountId = id;
            var acc = _accountsRepository.GetAccountById(id);
            viewModel.CurrentBalance = acc.Balance;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult New(TransactionNewViewModel viewModel)
        {
            if (!_accountsRepository.CheckIfSufficientBalance(viewModel.Amount, viewModel.CurrentBalance))
            {
                ModelState.AddModelError("Amount", "Insufficient funds. \n Amount cannot be greater than Balance");
            }
            if ( viewModel.selectedOperation == "Transfer to Another account" && _accountsRepository.GetAccountById(viewModel.ToAccountId) == null)
            {
                ModelState.AddModelError("ToAccountId", "Please enter a valid Account Id");
            }

            if (viewModel.AccountId == viewModel.ToAccountId)
            {
                ModelState.AddModelError("ToAccountId", "To Account can not be equal to Current Account");
            }

            if (ModelState.IsValid)
            {

                switch (viewModel.selectedOperation)
                {
                    case "Withdrawal in Cash":
                        _accountsRepository.Withdrawal(viewModel.AccountId, viewModel.Amount);
                        break;
                    case "Credit Card Withdrawal":
                        _accountsRepository.Withdrawal(viewModel.AccountId, viewModel.Amount);
                        break;
                    case "Collection From Another Bank":
                        _accountsRepository.Withdrawal(viewModel.AccountId, viewModel.Amount);
                        break;
                    case "Remittance to Another Bank":
                        _accountsRepository.Withdrawal(viewModel.AccountId, viewModel.Amount);
                        break;
                    case "Credit In Cash":
                        _accountsRepository.Deposit(viewModel.AccountId, viewModel.Amount);
                        break;
                    case "Credit":
                        _accountsRepository.Deposit(viewModel.AccountId, viewModel.Amount);
                        break;
                    case "Transfer to Another account":
                        _accountsRepository.Transfer(viewModel.AccountId, viewModel.ToAccountId, viewModel.Amount);
                        break;
                }
                _accountsRepository.NewTransaction(viewModel);


                return RedirectToAction("New", new { id = viewModel.AccountId, CurrentBalance = viewModel.CurrentBalance });

            }
            viewModel.Operations = GetOperationListItems();
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
            list.Add(new SelectListItem { Value = "Credit Card Withdrawal", Text = "Credit Card Withdrawal" });
            list.Add(new SelectListItem { Value = "Transfer to Another account", Text = "Transfer to Another account" });


            return list;
        }

    }
}
