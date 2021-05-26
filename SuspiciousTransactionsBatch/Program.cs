using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using SharedLibrary.data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SuspiciousTransactionsBatch
{
    public class Program
    {

        private static ServiceProvider _serviceProvider;

        private static void RegisterService()
        {
            var services = new ServiceCollection();

            services.AddTransient<ITransactionFinder, TransactionFinder>();

            services.AddDbContext<BankAppDataContext>(options =>
                options.UseSqlServer(
                    "Server=localhost;Database=BankAppData;Trusted_Connection=True;MultipleActiveResultSets=true"));

            _serviceProvider = services.BuildServiceProvider(true);
        }
        
        static void Main(string[] args)
        {

            RegisterService();
            var scope = _serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<ITransactionFinder>().Run();
        }
    }
}
