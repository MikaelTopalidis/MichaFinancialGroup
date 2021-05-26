using MichaFinancialGroup.Models;
using SharedLibrary.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.Services
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly BankAppDataContext _dbContext;
        public TransactionsRepository(BankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<Transactions> GetAll()
        {
            return _dbContext.Transactions;
        }

        public IQueryable<Transactions> GetTransactionsForChart()
        {
            var todaysDate = DateTime.Now;
            var tenDaysAgo = todaysDate.AddDays(-10);
            return _dbContext.Transactions.Where(d => d.Date > tenDaysAgo);
        } 
        
        public IQueryable<DateTime> GetTransactionDatesForChart()
        {
            var todaysDate = DateTime.Now;
            var tenDaysAgo = todaysDate.AddDays(-10);
            return _dbContext.Transactions.Select(d => d.Date).Where(d => d.Date > tenDaysAgo);
        }
    }
}
