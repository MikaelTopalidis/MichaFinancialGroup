 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MichaFinancialGroup.ViewModels;
using SharedLibrary.data;

namespace MichaFinancialGroup.Services
{
    public class AccountsRepository : IAccountsRepository
    {
        private readonly BankAppDataContext _dbContext;
        public AccountsRepository(BankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Accounts GetAccountById(int id)
        {
            return _dbContext.Accounts.Where(a => a.AccountId == id).FirstOrDefault();
            
        }

        public void Deposit(int id, decimal amount)
        {
            var account = GetAccountById(id);

            account.Balance += amount;
        }


        public void Transfer(int fromId, int toId, decimal amount)
        {
            Deposit(toId, amount);
            Withdrawal(fromId, amount);
        }

        public void Withdrawal(int id, decimal amount)
        {
            var account = GetAccountById(id);

            account.Balance -= amount;
           
        }

        public void NewTransaction(TransactionNewViewModel model)
        {
            var sender = GetAccountById(model.AccountId);
            var receiver = GetAccountById(model.ToAccountId);
            var creditTransaction = new Transactions();

            creditTransaction.AccountId = model.AccountId;
            creditTransaction.Account = model.Account;
            creditTransaction.Date = DateTime.Now.Date;
            creditTransaction.Type = "Credit";
            creditTransaction.Operation = model.selectedOperation;
            creditTransaction.Amount = model.Amount;
            creditTransaction.Balance = sender.Balance;
            creditTransaction.Symbol = "";

            if (model.ToAccountId != 0)
            {
                var debitTransaction = new Transactions();

                debitTransaction.AccountId = model.AccountId;
                debitTransaction.Account = model.Account;
                debitTransaction.Date = DateTime.Now.Date;
                debitTransaction.Type = "Debit";
                debitTransaction.Balance = receiver.Balance;
                debitTransaction.Operation = "Credit In Cash";
                debitTransaction.Amount = model.Amount;
                debitTransaction.Symbol = "";
                receiver.Transactions.Add(debitTransaction);

            }
            sender.Transactions.Add(creditTransaction);
            _dbContext.SaveChanges();

        }

        public bool CheckIfSufficientBalance(decimal amount, decimal balance)
        {
            if (amount > balance)
            {
                return false;
            }
            return true;
        }

    }
}
