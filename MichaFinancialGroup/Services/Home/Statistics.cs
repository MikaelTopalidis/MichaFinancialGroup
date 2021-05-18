using MichaFinancialGroup.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.Services
{
    public class Statistics : IStatistics
    {
        private readonly BankAppDataContext _dbContext;
        public Statistics(BankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Dispositions> GetAll()
        {
            return _dbContext.Dispositions.Include(a => a.Account).Include(c => c.Customer);

        }
    }
}
