using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.ViewModels.Transactions
{
    public class TransactionIndexViewModel
    {
        public IEnumerable<TransactionsViewModel> Transactions { get; set; }
        public string SortOrder { get; set; }
        public string SortField { get; set; }
        public string OppositeSortOrder { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public class TransactionsViewModel
        {
            public int TransactionId { get; set; }
            public int AccountId { get; set; }
            public DateTime Date { get; set; }
            public string Type { get; set; }
            public string Operation { get; set; }
            public decimal Amount { get; set; }
            public decimal Balance { get; set; }
            public string Symbol { get; set; }
            public string Bank { get; set; }
            public string Account { get; set; }
        }
    }
}
