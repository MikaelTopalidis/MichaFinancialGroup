using MichaFinancialGroup.Controllers;
using MichaFinancialGroup.Services;
using MichaFinancialGroup.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedLibrary.data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AutoFixture;

namespace MichaFinancialGroup.Tests.Controllers
{
    [TestClass]
    public class TransactionsControllerTests : BaseTest
    {
  
        private TransactionsController sut;
        private Mock<IAccountsRepository> accountRepositoryMock;
        private Mock<ITransactionsRepository> transactionsRepositoryMock;
        private BankAppDataContext ctx;
        public TransactionsControllerTests()
        {
            ctx = ctxInMemmory;
            accountRepositoryMock = new Mock<IAccountsRepository>();
            transactionsRepositoryMock = new Mock<ITransactionsRepository>();
            sut = new TransactionsController(transactionsRepositoryMock.Object, accountRepositoryMock.Object);
        }

        [TestMethod]
        public void WhenNewTransactionIsCalledAndBalaceIsSufficientANewTransactionShouldBeCreated()
        {
            var model = fixture.Create<TransactionNewViewModel>();

            accountRepositoryMock.Setup(e => e.CheckIfSufficientBalance(model.Amount, model.CurrentBalance)).Returns(true);
            sut.New(model);

            accountRepositoryMock.Verify(e => e.NewTransaction(model), Times.Once);
        } 
        
        [TestMethod]
        public void WhenNewTransactionIsCalledAndBalaceIsNOTSufficientANewTransactionShouldNoBeCreated()
        {
            var model = fixture.Create<TransactionNewViewModel>();

            accountRepositoryMock.Setup(e => e.CheckIfSufficientBalance(model.Amount, model.CurrentBalance)).Returns(false);
            sut.New(model);

            accountRepositoryMock.Verify(e => e.NewTransaction(model), Times.Never);
        }



        [TestMethod]
        public void IfAmountIsNegativeANewTransactionShouldNotBeCreated()
        {
            var model = fixture.Create<TransactionNewViewModel>();
            model.Amount = -100;

            accountRepositoryMock.Setup(e => e.Withdrawal(model.AccountId, model.Amount));
            accountRepositoryMock.Setup(e => e.Transfer(model.AccountId, model.ToAccountId, model.Amount));
            accountRepositoryMock.Setup(e => e.Deposit(model.AccountId, model.Amount));
            sut.New(model);
            accountRepositoryMock.Verify(e=>e.NewTransaction(model), Times.Never);
        }
        
        
        
        
        [TestMethod]
        public void ModelstateIsInvalidIfAmountIsGreaterThanBalance()
        {
            var model = new TransactionNewViewModel { Amount = 100, CurrentBalance = 1 };

            var ctx = new ValidationContext(model, null, null);
            var result = new List<ValidationResult>();
            var valid = Validator.TryValidateObject(model, ctx, result, true);

            Assert.IsFalse(valid);
        }

        [TestMethod]
        public void ModelstateIsInvalidIfAmountIsNegative()
        {
            var model = new TransactionNewViewModel { Amount = -100, CurrentBalance = 1 };

            var ctx = new ValidationContext(model, null, null);
            var result = new List<ValidationResult>();
            var valid = Validator.TryValidateObject(model, ctx, result, true);

            Assert.IsFalse(valid);
        }
    }
}
