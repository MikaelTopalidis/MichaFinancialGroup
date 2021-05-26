using MichaFinancialGroup.Models;
using SharedLibrary.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace MichaFinancialGroup.Services
{
    public interface ITransactionsRepository
    {
        public IQueryable<Transactions> GetAll();
        public IQueryable<Transactions> GetTransactionsForChart();
        public IQueryable<DateTime> GetTransactionDatesForChart();
    }
}
