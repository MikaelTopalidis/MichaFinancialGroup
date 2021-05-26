using MichaFinancialGroup.Models;
using SharedLibrary.data;
using System.Collections.Generic;
using System.Linq;

namespace MichaFinancialGroup.Services
{
    public interface ICustomerRepository
    {
        IQueryable<Customers> GetCustomers(string q, string sortField, string sortOrder, int page, int pageSize);
        IEnumerable<Dispositions> GetAll(string country);
        IEnumerable<Transactions> GetTransactionsForCustomer(int id);
        Customers GetCustomerDetails(int id);
        IEnumerable<Customers> GetTopTen(string country);
        public bool CheckIfValidCustomerId(int id);
        void AddCustomer(Customers dbCustomer);
        Customers GetCustomerById(int id);
        void UpdateAzure(Customers customer);
        void UpdateCustomer(Customers dbCustomer);
        void AddDispositionToAccount(Dispositions newDispositon, Accounts newAccounts);
        void AddDispositionToCustomer(Dispositions newDispositon, Customers newCustomer);
    }


}
