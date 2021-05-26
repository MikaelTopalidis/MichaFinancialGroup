using AutoFixture;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.data;
using System;
using System.Linq;

namespace MichaFinancialGroup.Tests
{
    public class BaseTest
    {
        protected AutoFixture.Fixture fixture = new AutoFixture.Fixture();
        public BankAppDataContext ctxInMemmory;

        public BaseTest()
        {
            var options = new DbContextOptionsBuilder<BankAppDataContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .EnableSensitiveDataLogging()
               .Options;

            ctxInMemmory = new BankAppDataContext(options);

            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));


        }
    }
}
