using MichaFinancialGroup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.Services
{
    public interface ICustomerRepository
    {
        IEnumerable<Customers> GetCustomers();
        IEnumerable<Dispositions> GetAll(string country);
        IEnumerable<Transactions> GetTransactionsForCustomer(int id);
        Customers GetCustomerDetails(int id);
        IEnumerable<Customers> GetTopTen(string country);
        
    }

    
}
