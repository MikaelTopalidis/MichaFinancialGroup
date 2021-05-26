using AutoFixture;
using MichaFinancialGroup.Controllers;
using MichaFinancialGroup.Services;
using MichaFinancialGroup.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedLibrary.data;
using System.Collections.Generic;

namespace MichaFinancialGroup.Tests.Controllers
{
    [TestClass]
    public class CustomerControllerTests : BaseTest
    {
        private CustomerController sut;
        private Mock<ICustomerRepository> customersRepositoryMock;
        private BankAppDataContext ctx;
        public CustomerControllerTests()
        {
            ctx = ctxInMemmory;
            customersRepositoryMock = new Mock<ICustomerRepository>();
            sut = new CustomerController(customersRepositoryMock.Object);

        }

        [TestMethod]
        public void CheckIfActionDetailsReturnsViewResult()
        {
            var customer = fixture.Create<Customers>();
            customersRepositoryMock.Setup(c => c.GetCustomerDetails(1)).Returns(customer);
            var result = sut.Details(1);

            Assert.IsNotNull(result);
          
        }

        [TestMethod]
        public void CheckIfDbcustomerEqualsModelcustomer()
        {
            var id = fixture.Create<int>();
            var fixcustomer = fixture.Create<Customers>();

            customersRepositoryMock.Setup(e => e.GetCustomerDetails(id)).Returns(fixcustomer);

            var customer = customersRepositoryMock.Object.GetCustomerDetails(id);

            var result = sut.Details(id);
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as CustomerDetailsViewModel;

            Assert.AreEqual(customer.Surname, model.Surname);
            Assert.AreEqual(customer.Givenname, model.Givenname);
            Assert.AreEqual(customer.CountryCode, model.CountryCode);
            Assert.AreEqual(customer.Emailaddress, model.Emailaddress);
            Assert.AreEqual(customer.City, model.City);
            Assert.AreEqual(customer.Gender, model.Gender);
            Assert.AreEqual(customer.CustomerId, model.CustomerId);
            Assert.AreEqual(customer.Birthday, model.Birthday);
            Assert.AreEqual(customer.NationalId, model.NationalId);
            Assert.AreEqual(customer.Streetaddress, model.Streetaddress);
            Assert.AreEqual(customer.Zipcode, model.Zipcode);
            Assert.AreEqual(customer.Country, model.Country);
            Assert.AreEqual(customer.Telephonecountrycode, model.Telephonecountrycode);
            Assert.AreEqual(customer.Telephonenumber, model.Telephonenumber);
        }
    }
}
