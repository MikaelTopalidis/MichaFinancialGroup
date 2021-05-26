using MichaFinancialGroup.Services;
using MichaFinancialGroup.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using AutoFixture;
using SharedLibrary.data;
using System.Collections.Generic;
using System;

namespace MichaFinancialGroup.Tests.Controllers
{
    [TestClass]
    public class AccountRepositoryTests : BaseTest
    {

        private AccountsRepository sut;
        private BankAppDataContext ctx;

        public AccountRepositoryTests()
        {
            ctx = ctxInMemmory;
            sut = new AccountsRepository(ctx);
        }

        [TestMethod]
        public void CheckIfCorrectValuesInDatabaseWhenNewTransactionIsValid()
        {
            var model = fixture.Create<TransactionNewViewModel>();
            model.Date = DateTime.Now.Date;
            model.Symbol = "";
            model.Account = "";
            model.selectedOperation = "Credit In Cash";

            var account = new Accounts
            {
                AccountId = model.AccountId,
                Frequency = "Monthly",
                Transactions = new List<Transactions>(),
                Balance = model.CurrentBalance,
                Created = new DateTime(2021, 01, 01),
                Dispositions = new List<Dispositions>()
            };

            var account1 = new Accounts
            {
                AccountId = model.ToAccountId,
                Frequency = "Monthly",
                Transactions = new List<Transactions>(),
                Balance = model.CurrentBalance,
                Created = new DateTime(2021, 01, 01),
                Dispositions = new List<Dispositions>()
            };

            ctx.Accounts.Add(account); 
            
            ctx.Accounts.Add(account1);
           
            ctx.SaveChanges();


            sut.NewTransaction(model);
            var sender =  sut.GetAccountById(account.AccountId);
            var receiver = sut.GetAccountById(account1.AccountId);


            Assert.AreEqual(model.Amount, ctx.Transactions.FirstOrDefault(t => t.AccountId == sender.AccountId).Amount);
            Assert.AreEqual(model.AccountId, ctx.Transactions.FirstOrDefault(t => t.AccountId == sender.AccountId).AccountId);
            Assert.AreEqual(model.selectedOperation, ctx.Transactions.FirstOrDefault(t => t.AccountId == sender.AccountId).Operation);
            Assert.AreEqual(model.Date, ctx.Transactions.FirstOrDefault(t => t.AccountId == sender.AccountId).Date);
            Assert.AreEqual(model.Symbol, ctx.Transactions.FirstOrDefault(t => t.AccountId == sender.AccountId).Symbol);
            Assert.AreEqual(model.CurrentBalance, ctx.Transactions.FirstOrDefault(t => t.AccountId == sender.AccountId).Balance);
            Assert.AreEqual(model.Account, ctx.Transactions.FirstOrDefault(t => t.AccountId == sender.AccountId).Account);

            Assert.AreEqual(model.Amount, ctx.Transactions.FirstOrDefault(t => t.AccountId == receiver.AccountId).Amount);
            Assert.AreEqual(model.ToAccountId, ctx.Transactions.FirstOrDefault(t => t.AccountId == receiver.AccountId).AccountId);
            Assert.AreEqual(model.selectedOperation, ctx.Transactions.FirstOrDefault(t => t.AccountId == receiver.AccountId).Operation);
            Assert.AreEqual(model.Date, ctx.Transactions.FirstOrDefault(t => t.AccountId == receiver.AccountId).Date);
            Assert.AreEqual(model.Symbol, ctx.Transactions.FirstOrDefault(t => t.AccountId == receiver.AccountId).Symbol);
            Assert.AreEqual(model.CurrentBalance, ctx.Transactions.FirstOrDefault(t => t.AccountId == receiver.AccountId).Balance);
            Assert.AreEqual(model.Account, ctx.Transactions.FirstOrDefault(t => t.AccountId == receiver.AccountId).Account);
        }
    }
}
