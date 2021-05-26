using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using MichaFinancialGroup.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibrary.data;
using SharedLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IConfiguration _config;
        private readonly BankAppDataContext _dbContext;
        public CustomerRepository(BankAppDataContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }
        public IQueryable<Customers> GetCustomers(string q, string sortField, string sortOrder, int page, int pageSize)
        {

            var customersToSkip = pageSize * (page - 1);
            var _searchClient = new SearchClient(new Uri(_config["AzureSearch:searchUrl"]), _config["AzureSearch:indexName"], new AzureKeyCredential(_config["AzureSearch:Key"]));
            var searchOptions = new SearchOptions
            {
                OrderBy = { $"{sortField}  {sortOrder}" },
                Skip = customersToSkip,
                Size = pageSize,
            };
            var searchResult = _searchClient.Search<CustomerInAzure>(q+"*", searchOptions);

            var customerIdsInResult = new List<int>();
            foreach (var customer in searchResult.Value.GetResults())
            {
                customerIdsInResult.Add(Convert.ToInt32(customer.Document.Id));
            }

            var customersToDisplay = _dbContext.Customers.Where(c => customerIdsInResult.Contains(c.CustomerId));

            return customersToDisplay;
        }
        
        public Customers GetCustomerDetails(int id)
        {

            return _dbContext.Customers.Include(d=>d.Dispositions).ThenInclude(a=>a.Account).FirstOrDefault(c=>c.CustomerId == id);
        }
        
        public IEnumerable<Transactions> GetTransactionsForCustomer(int id)
        {

            return _dbContext.Transactions.Where(c => c.AccountId == id);
        }

        public IEnumerable<Customers> GetTopTen(string country)
        {
            return _dbContext.Customers.Include(d => d.Dispositions).ThenInclude(a => a.Account).Where(c => c.Country == country);
        }

        public IEnumerable<Dispositions> GetAll(string country)
        {
            return _dbContext.Dispositions.Include(d => d.Account).Include(D => D.Customer)
                .Where(d => d.Customer.Country == country);
        }

        public bool CheckIfValidCustomerId(int id)
        {
            if (_dbContext.Customers.Any(c=>c.CustomerId == id))
            {
                return true;
            }
            return false;
        }

        public void AddCustomer(Customers dbCustomer)
        {
            _dbContext.Customers.Add(dbCustomer);
            _dbContext.SaveChanges();
        }

        public Customers GetCustomerById(int id)
        {
            return _dbContext.Customers.First(c => c.CustomerId == id);
        }

        public void UpdateAzure(Customers customer)
        {
            var searchClient = new SearchClient(new Uri(_config["AzureSearch:searchUrl"]), _config["AzureSearch:indexName"], new AzureKeyCredential(_config["AzureSearch:Key"]));

            var batch = new IndexDocumentsBatch<CustomerInAzure>();

            var customerIndex = new CustomerInAzure
            {
                Id = customer.CustomerId.ToString(),
                City = customer.City,
                Name = customer.Givenname + " " + customer.Surname,
                PersonalNumber = customer.NationalId,
                Address = customer.Streetaddress + ", " + customer.Zipcode,
            };

            batch.Actions.Add(new IndexDocumentsAction<CustomerInAzure>(IndexActionType.MergeOrUpload, customerIndex));
            IndexDocumentsResult result = searchClient.IndexDocuments(batch);
        }

        public void UpdateCustomer(Customers dbCustomer)
        {
            _dbContext.Customers.Update(dbCustomer);
            _dbContext.SaveChanges();
        }

        public void AddDispositionToAccount(Dispositions newDispositon, Accounts newAccount)
        {
            newAccount.Dispositions.Add(newDispositon);
            _dbContext.SaveChanges();

        }

        public void AddDispositionToCustomer(Dispositions newDispositon, Customers newCustomer)
        {
            newCustomer.Dispositions.Add(newDispositon);
            _dbContext.SaveChanges();
        }
    }
}
