using MichaFinancialGroup.Services;
using MichaFinancialGroup.ViewModels;
using MichaFinancialGroup.ViewModels.Customer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public IActionResult Index(string q, string sortField, string sortOrder, int page = 1)
        {
            var viewModel = new CustomerIndexViewModel();

                var query = _customerRepository.GetCustomers()
               .Where(r => q == null || r.Givenname.Contains(q) ||r.Surname.Contains(q) || r.City.Contains(q) || r.CustomerId.ToString().Contains(q));


            //ANTAL POSTER SOM MATCHAR FILTRET
            int totalRowCount = query.Count();

            if (string.IsNullOrEmpty(sortField))
                sortField = "Name";
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "asc";

            if (sortField == "Name")
            {
                if (sortOrder == "asc")
                    query = query.OrderBy(y => y.Givenname);
                else
                    query = query.OrderByDescending(y => y.Givenname);
            }

            if (sortField == "Address")
            {
                if (sortOrder == "asc")
                    query = query.OrderBy(y => y.Streetaddress);
                else
                    query = query.OrderByDescending(y => y.Streetaddress);
            }

            if (sortField == "ssn")
            {
                if (sortOrder == "asc")
                    query = query.OrderBy(y => y.Birthday);
                else
                    query = query.OrderByDescending(y => y.Birthday);

            }

            int pageSize = 10;

            var pageCount = (double)totalRowCount / pageSize;
            viewModel.TotalPages = (int)Math.Ceiling(pageCount);


        //Skip - hoppa över så många
        //Take - sen ta så många

        int howManyRecordsToSkip = (page - 1) * pageSize;  // Sida 1 ->  0

            query = query.Skip(howManyRecordsToSkip).Take(pageSize);


            viewModel.Customers = query.Select(dbCustomer => new CustomerIndexViewModel.CustomerViewModel
            {
                CustomerId = dbCustomer.CustomerId,
                FullName = dbCustomer.Givenname + " " + dbCustomer.Surname,
                PersonalNumber = dbCustomer.NationalId,
                Address = dbCustomer.Streetaddress + ", " + dbCustomer.Zipcode,
                City = dbCustomer.City
            }).ToList();

            viewModel.q = q;
            viewModel.SortOrder = sortOrder;
            viewModel.SortField = sortField;
            viewModel.Page = page;
            viewModel.OppositeSortOrder = sortOrder == "asc" ? "desc" : "asc";

           

            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            
            var query = _customerRepository.GetCustomerDetails(id);
            
            var viewModel = new CustomerDetailsViewModel
            {
                CustomerId =  query.CustomerId,
                NationalId = query.NationalId,
                Birthday = (DateTime)query.Birthday,
                Country = query.Country,
                Telephonecountrycode = query.Telephonecountrycode,
                Telephonenumber = query.Telephonenumber,
                Surname = query.Surname,
                Emailaddress = query.Emailaddress,
                Gender = query.Gender,
                Zipcode = query.Zipcode,
                CountryCode = query.CountryCode,
                Streetaddress = query.Streetaddress,
                Givenname = query.Givenname,
                City = query.City,
                Dispositions = query.Dispositions.Select(a => new CustomerDetailsViewModel.DispositionsViewModel
                {
                    Account = new CustomerDetailsViewModel.AccountsViewModel
                    {
                        AccountId = a.AccountId,
                        Type = a.Type,
                        Balance = a.Account.Balance,
                        Created = a.Account.Created,
                        Frequency = a.Account.Frequency
                    }
                }).ToList()

            };

            viewModel.TotalBalance = query.Dispositions.Select(c => c.Account.Balance).Sum();

            return View(viewModel);
        }

        public IActionResult Transactions(int id, string q, string sortField, string sortOrder, int page = 1)
        {
            var viewModel = new CustomerTransactionsViewModel();
            viewModel.CustomerId = id;
            var query = _customerRepository.GetTransactionsForCustomer(id);

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


            viewModel.Transactions = query.Skip(0).Take(20).Select(dbTransact => new CustomerTransactionsViewModel.TransactionsViewModel
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


            viewModel.q = q;
            viewModel.SortOrder = sortOrder;
            viewModel.SortField = sortField;
            viewModel.Page = page;
            viewModel.OppositeSortOrder = sortOrder == "asc" ? "desc" : "asc";
            return View(viewModel);
        }

        public IActionResult LoadTransactions(string sortField, string sortOrder, int id, int skip, int page = 1)
        {
            var viewModel = new CustomerTransactionsViewModel.TransactionsLoadTransactionsViewModel();
            var query = _customerRepository.GetTransactionsForCustomer(id);

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

            viewModel.Transactions = query.Skip(skip).Take(20).Select(dbTransact => new CustomerTransactionsViewModel.TransactionsViewModel
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

        public IActionResult TopTen(string country)
        {
            var query = _customerRepository.GetAll(country);

            var viewModel = new CustomerTopTenViewModel();

            viewModel.Customers = query.OrderByDescending(b=>b.Account.Balance).Take(10).Select(c => new CustomerTopTenViewModel.TopTenViewModel
            {
                Id = c.CustomerId,
                Name = c.Customer.Givenname + c.Customer.Surname,
                TotalBalance = c.Account.Balance
            }).ToList();

            return View();
        }
    }
}
