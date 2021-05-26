using System;
using System.Collections.Generic;
using System.Text;

namespace SuspiciousTransactionsBatch
{
    public class TransactionsByCountry
    {
        public string Country { get; set; }
        public List<SuspectedCustomer> suspcustmers { get; set; } = new List<SuspectedCustomer>();
    }
}
