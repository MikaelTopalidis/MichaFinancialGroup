using MichaFinancialGroup.Services;
using MichaFinancialGroup.ViewModels;
using MichaFinancialGroup.ViewModels.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharedLibrary.data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MichaFinancialGroup.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [Authorize(Roles = "Admin, Cashier")]
        public IActionResult Index(string q, string sortField, string sortOrder, int page = 1)
        {
            var viewModel = new CustomerIndexViewModel();
            int pageSize = 50;

            var isNumeric = int.TryParse(q, out int n);

            if (_customerRepository.CheckIfValidCustomerId(n))
            {
                return RedirectToAction("details", new { id = n });
            }

            if (string.IsNullOrEmpty(sortField))
                sortField = "Name";
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "asc";
            var query = _customerRepository.GetCustomers(q, sortField, sortOrder, page, pageSize);


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

            if (sortField == "Id")
            {
                if (sortOrder == "asc")
                    query = query.OrderBy(y => y.Streetaddress);
                else
                    query = query.OrderByDescending(y => y.Streetaddress);
            }

            if (sortField == "PersonalNumber")
            {
                if (sortOrder == "asc")
                    query = query.OrderBy(y => y.Birthday);
                else
                    query = query.OrderByDescending(y => y.Birthday);

            }

            if (sortField == "City")
            {
                if (sortOrder == "asc")
                    query = query.OrderBy(y => y.Birthday);
                else
                    query = query.OrderByDescending(y => y.Birthday);

            }


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

        [Authorize(Roles = "Admin, Cashier")]
        public IActionResult NewAccount(int Id)
        {
            var customer = _customerRepository.GetCustomerById(Id);
            var newAccount = new Accounts();
            newAccount.Created = DateTime.Now.Date;
            newAccount.Balance = 0;
            newAccount.Frequency = "Monthly";

            var newDispositon = new Dispositions();
            newDispositon.Type = "OWNER";
            newDispositon.Account = newAccount;
            newDispositon.Customer = customer;
            _customerRepository.AddDispositionToAccount(newDispositon, newAccount);
            _customerRepository.AddDispositionToCustomer(newDispositon, customer);

            return RedirectToAction("details", new { id = Id });
        }

        [Authorize(Roles = "Admin, Cashier")]
        public IActionResult New()
        {
            var viewModel = new CustomerNewViewModel();
            viewModel.Genders = GetGendersListItems();
            viewModel.Countries = GetCountriesListItems();
            viewModel.Birthday = new DateTime(1900,01,01);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult New(CustomerNewViewModel viewModel)
        {
            if (viewModel.SelectedGender == "0")
            {
                ModelState.AddModelError("SelectedGender", "Please select a gender");
            }
            if (viewModel.SelectedCountry == "0")
            {
                ModelState.AddModelError("SelectedCountry", "Please select a country");
            }

            if (ModelState.IsValid)
            {
                if (viewModel.SelectedCountry == "Sweden")
                {
                    viewModel.CountryCode = "SE";
                }
                if (viewModel.SelectedCountry == "Norway")
                {
                    viewModel.CountryCode = "NO";
                }
                if(viewModel.SelectedCountry == "Finland")
                {
                    viewModel.CountryCode = "FI";
                }
                var newCustomer = new Customers
                {
                    NationalId = viewModel.NationalId,
                    Givenname = viewModel.Givenname,
                    Birthday = viewModel.Birthday,
                    City = viewModel.City,
                    Country = viewModel.SelectedCountry,
                    Gender = viewModel.SelectedGender,
                    CountryCode = viewModel.CountryCode,
                    Emailaddress = viewModel.Emailaddress,
                    Streetaddress = viewModel.Streetaddress,
                    Surname = viewModel.Surname,
                    Telephonecountrycode = viewModel.Telephonecountrycode,
                    Telephonenumber = viewModel.Telephonenumber,
                    Zipcode = viewModel.Zipcode
                    
                };
                _customerRepository.AddCustomer(newCustomer);

                var newAccount = new Accounts();
                newAccount.Created = DateTime.Now.Date;
                newAccount.Balance = 0;
                newAccount.Frequency = "Monthly";

                var newDispositon = new Dispositions();
                newDispositon.Type = "OWNER";
                newDispositon.Account = newAccount;
                newDispositon.Customer = newCustomer;

                _customerRepository.AddDispositionToAccount(newDispositon, newAccount);
                _customerRepository.AddDispositionToCustomer(newDispositon, newCustomer);
                _customerRepository.UpdateAzure(newCustomer);

                return RedirectToAction("Index");
            }
            viewModel.Genders = GetGendersListItems();
            viewModel.Countries = GetCountriesListItems();
            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Cashier")]
        public IActionResult Edit(int Id)
        {
            var viewModel = new CustomerEditViewModel();
            viewModel.Genders = GetGendersListItems();
            viewModel.Countries = GetCountriesListItems();

            var dbCustomer = _customerRepository.GetCustomerById(Id);

            viewModel.Givenname = dbCustomer.Givenname;
            viewModel.Surname = dbCustomer.Surname;
            viewModel.NationalId = dbCustomer.NationalId;
            viewModel.SelectedGender = dbCustomer.Gender;
            viewModel.Streetaddress = dbCustomer.Streetaddress;
            viewModel.Telephonecountrycode = dbCustomer.Telephonecountrycode;
            viewModel.Telephonenumber = dbCustomer.Telephonenumber;
            viewModel.Zipcode = dbCustomer.Zipcode;
            viewModel.CountryCode = dbCustomer.CountryCode;
            viewModel.SelectedCountry = dbCustomer.Country;
            viewModel.Birthday = dbCustomer.Birthday;
            viewModel.Emailaddress = dbCustomer.Emailaddress;
            viewModel.City = dbCustomer.City;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(int Id, CustomerEditViewModel viewModel)
        {
            if (viewModel.SelectedGender == "0")
            {
                ModelState.AddModelError("SelectedGender", "Please select a gender");
            }
            if (viewModel.SelectedCountry == "0")
            {
                ModelState.AddModelError("SelectedCountry", "Please select a country");
            }

            if (ModelState.IsValid)
            {
                var dbCustomer = _customerRepository.GetCustomerById(Id);
                if (viewModel.SelectedCountry == "Sweden")
                {
                    viewModel.CountryCode = "SE";
                }
                if (viewModel.SelectedCountry == "Norway")
                {
                    viewModel.CountryCode = "NO";
                }
                if(viewModel.SelectedCountry == "Finland")
                {
                    viewModel.CountryCode = "FI";
                }
                dbCustomer.Givenname = viewModel.Givenname;
                dbCustomer.Surname =viewModel.Surname;
                dbCustomer.NationalId = viewModel.NationalId;
                dbCustomer.Gender = viewModel.SelectedGender;
                dbCustomer.Streetaddress = viewModel.Streetaddress;
                dbCustomer.Telephonecountrycode = viewModel.Telephonecountrycode;
                dbCustomer.Telephonenumber = viewModel.Telephonenumber;
                dbCustomer.Zipcode = viewModel.Zipcode;
                dbCustomer.CountryCode = viewModel.CountryCode;
                dbCustomer.Country = viewModel.SelectedCountry;
                dbCustomer.Birthday = viewModel.Birthday;
                dbCustomer.City = viewModel.City;

                _customerRepository.UpdateCustomer(dbCustomer);
                _customerRepository.UpdateAzure(dbCustomer);
                return RedirectToAction("Index");
            }

            viewModel.Genders = GetGendersListItems();
            viewModel.Countries = GetCountriesListItems();
            return View(viewModel);
        }

        private List<SelectListItem> GetGendersListItems()
        {

            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "0", Text = "Select Gender" });
            list.Add(new SelectListItem { Value = "Female", Text = "Female" });
            list.Add(new SelectListItem { Value = "Male", Text = "Male" });

            return list;
        } 
        
        private List<SelectListItem> GetCountriesListItems()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "0", Text = "Select Country" });
            list.Add(new SelectListItem { Value = "Sweden", Text = "Sweden" });
            list.Add(new SelectListItem { Value = "Norway", Text = "Norway" });
            list.Add(new SelectListItem { Value = "Finland", Text = "Finland" });

            return list;
        }

        [Authorize(Roles = "Admin, Cashier")]
        public IActionResult Details(int id)
        {

            var query = _customerRepository.GetCustomerDetails(id);

            var viewModel = new CustomerDetailsViewModel
            {
                CustomerId = query.CustomerId,
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

        [Authorize(Roles = "Admin, Cashier")]
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

        [Authorize(Roles = "Admin, Cashier")]
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

        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "country" }, Location = ResponseCacheLocation.Any)]
        public IActionResult TopTen(string country)
        {
            var query = _customerRepository.GetAll(country);

            var viewModel = new CustomerTopTenViewModel();
            viewModel.Country = country;

            viewModel.Customers = query.OrderByDescending(b => b.Account.Balance).Where(a => a.Type == "OWNER").Take(10).Select(c => new CustomerTopTenViewModel.TopTenViewModel
            {
                Id = c.CustomerId,
                Name = c.Customer.Givenname + " " + c.Customer.Surname,
                TotalBalance = c.Account.Balance
            }).ToList();

            return View(viewModel);
        }
    }
}
