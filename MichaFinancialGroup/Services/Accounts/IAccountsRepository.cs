using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MichaFinancialGroup.ViewModels;
using SharedLibrary.data;

namespace MichaFinancialGroup.Services
{
    public interface IAccountsRepository
    {
        public Accounts GetAccountById(int id);
        public void Withdrawal(int id, decimal amount);
        public void Deposit(int id, decimal amount);
        public void Transfer(int fromId, int toId, decimal amount);
        public void NewTransaction(TransactionNewViewModel model);
        public bool CheckIfSufficientBalance(decimal amount, decimal balance);
    }
}
