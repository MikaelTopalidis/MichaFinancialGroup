using MichaFinancialGroup.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly BankAppDataContext _dbContext;
        public CustomerRepository(BankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<Customers> GetCustomers()
        {
            return _dbContext.Customers;
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
    }
}
