using System;
using System.Collections.Generic;
using System.Text;

namespace SuspiciousTransactionsBatch
{
    public class SuspectedCustomer
    {
        public int AccountId { get; set; }
        public int CustomerId { get; set; }
        public string AccountOwner { get; set; }
        public string Country { get; set; }
        public List<SuspectedTransactions> Transactionsa { get; set; } = new List<SuspectedTransactions>();

    }
            public class SuspectedTransactions
            {
                public decimal Amount { get; set; }
                public DateTime date { get; set; }
                public int TransactionId { get; set; }
            }
}
