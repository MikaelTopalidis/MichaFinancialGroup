using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.ViewModels
{
    public class CustomerTransactionsViewModel
    {
        public string q { get; set; }
        public List<TransactionsViewModel> Transactions { get; set; }
        public string SortOrder { get; set; }
        public string SortField { get; set; }
        public string OppositeSortOrder { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int CustomerId { get; set; }


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

        public class TransactionsLoadTransactionsViewModel
        {
            public string q { get; set; }
            public List<TransactionsViewModel> Transactions { get; set; }
            public string SortOrder { get; set; }
            public string SortField { get; set; }
            public string OppositeSortOrder { get; set; }
            public int Page { get; set; }
            public int TotalPages { get; set; }
            public int CustomerId { get; set; }
        }

    }
}
