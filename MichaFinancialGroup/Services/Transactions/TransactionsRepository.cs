using MichaFinancialGroup.Models;
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

        public IEnumerable<Transactions> GetAll()
        {
            return _dbContext.Transactions;
        }
    }
}
