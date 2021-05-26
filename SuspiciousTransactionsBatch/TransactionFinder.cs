using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using SharedLibrary.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuspiciousTransactionsBatch
{
    public class TransactionFinder : ITransactionFinder
    {
        private string _from = "no-reply@mfg.com";
        private string _to = "@mfg.com";
        private string _SmtpServer = "smtp.mailtrap.io";
        private int _port = 587;
        private string _SmtpUsername = "b8dd9e39f9f689";
        private string _SmtpPassword = "d8574fc839f6fb";
        private readonly BankAppDataContext _context;
        public TransactionFinder(BankAppDataContext context)
        {
            _context = context;
        }

        public void Run()
        {

            var sweden = new TransactionsByCountry();
            sweden.Country = "Sweden";
            var norway = new TransactionsByCountry();
            norway.Country = "Norway";
            var denmark = new TransactionsByCountry();
            denmark.Country = "Denmark";
            var finland = new TransactionsByCountry();
            finland.Country = "Finland";

            var suspiciousAccounts = GetAccountIdForSuspiciousTransactions();
            var suspicousCustomers = GetSuspectedAccountsWithTransactionsForCountry(suspiciousAccounts, DateTime.Now.Date);
            var textbody = "";
            foreach (var suspicousCustomer in suspicousCustomers)
            {
                textbody = ($"{suspicousCustomer.CustomerId} - {suspicousCustomer.AccountOwner} - {suspicousCustomer.Country}");
                textbody += ($"Transactions for Account {suspicousCustomer.AccountId}:");
                foreach (var suspicousCustomerFlagedTransaction in suspicousCustomer.Transactionsa)
                {
                    textbody += ($"{suspicousCustomerFlagedTransaction.TransactionId} - {suspicousCustomerFlagedTransaction.date} - {suspicousCustomerFlagedTransaction.Amount}");
                }

            }

            foreach (var customer in suspicousCustomers)
            {
                if (customer.Country == norway.Country)
                    norway.suspcustmers.Add(customer);

                if (customer.Country == sweden.Country)
                    sweden.suspcustmers.Add(customer);

                if (customer.Country == denmark.Country)
                    denmark.suspcustmers.Add(customer);

                if (customer.Country == finland.Country)
                    finland.suspcustmers.Add(customer);
            }

            if (sweden.suspcustmers.Any())
                SendReport(sweden, textbody);

            if (norway.suspcustmers.Any())
                SendReport(norway, textbody);

            if (denmark.suspcustmers.Any())
                SendReport(denmark, textbody);

            if (finland.suspcustmers.Any())
                SendReport(finland, textbody);
        }

        public List<int> GetAccountIdForSuspiciousTransactions()
        {
            var yesterDay = DateTime.Today.Date.AddDays(-1);
            var threeDaysAgo = DateTime.Today.AddDays(-3);
            var accIdForTransactionsOver15 = _context.Transactions.Where(t => t.Date == yesterDay && t.Amount > 15000).Select(t => t.AccountId).ToList();
            var accIdForTransactionsOver23 = _context.Transactions.Where(t => t.Date >= threeDaysAgo && t.Date <= yesterDay)
                .GroupBy(t => t.AccountId).Where(g => 23000 < g.Sum(a => a.Balance)).Select(a => a.Key).ToList();

            var list = new List<int>();

            list.AddRange(accIdForTransactionsOver15);
            list.AddRange(accIdForTransactionsOver23);

            return list.Distinct().ToList();
        }

        public List<SuspectedCustomer> GetSuspectedAccountsWithTransactionsForCountry(List<int> accounts, DateTime today)
        {
            var yesterday = today.Date.AddDays(-1);
            var threeDaysAgo = today.Date.AddDays(-3);
            var list = new List<SuspectedCustomer>();

            foreach (var account in accounts)
            {
                var dispostion = _context.Dispositions.Include(d => d.Account).ThenInclude(a => a.Transactions)
                    .Include(d => d.Customer).FirstOrDefault(d => d.Type == "OWNER" && d.AccountId == account);

                var flaggedTransactions =
                    dispostion.Account.Transactions.Where(t => t.Date == yesterday && 15000 < t.Amount);

                if (!flaggedTransactions.Any())
                    flaggedTransactions = dispostion.Account.Transactions
                        .Where(t => threeDaysAgo <= t.Date && t.Date <= yesterday);

                var customer = new SuspectedCustomer
                {
                    AccountId = dispostion.AccountId,
                    CustomerId = dispostion.CustomerId,
                    AccountOwner = $"{dispostion.Customer.Surname}, {dispostion.Customer.Givenname}",
                    Country = dispostion.Customer.Country,
                    Transactionsa = flaggedTransactions.Select(f => new SuspectedTransactions
                    {
                        Amount = f.Amount,
                        date = f.Date,
                        TransactionId = f.TransactionId
                    }).ToList()

                };

                list.Add(customer);
            }

            return list;
        }

        private void SendReport(TransactionsByCountry transactions, string textbody)
        {

            var emailConfig = new EmailConfiguration
            {
                SmtpPassword = _SmtpPassword,
                SmtpPort = _port,
                SmtpServer = _SmtpServer,
                SmtpUsername = _SmtpUsername

            };

            var report = new MimeMessage();
            var bodyBuilder = new BodyBuilder();

            // From
            report.From.Add(new MailboxAddress("Micha Finanacial Group", _from));
            // To
            report.To.Add(new MailboxAddress("Micha Finanacial Group", transactions.Country + _to)); // Testadress: f9bee8f981-50f925@inbox.mailtrap.io
            // Reply to
            report.ReplyTo.Add(new MailboxAddress("Micha Finanacial Group", "admin@mfg.com"));

            report.Subject = $"Suspect transactions for {transactions.Country}";
            bodyBuilder.HtmlBody = textbody;
            report.Body = bodyBuilder.ToMessageBody();

            using (var emailClient = new SmtpClient())
            {

                emailClient.Connect(emailConfig.SmtpServer, emailConfig.SmtpPort, SecureSocketOptions.Auto);

                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(emailConfig.SmtpUsername, emailConfig.SmtpPassword);

                emailClient.Send(report);

                emailClient.Disconnect(true);
            }
        }
    }
}
