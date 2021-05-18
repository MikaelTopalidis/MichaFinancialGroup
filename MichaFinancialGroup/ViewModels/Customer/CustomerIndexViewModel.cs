using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.ViewModels
{
    public class CustomerIndexViewModel
    {
            public string q { get; set; }
            public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
            public string SortOrder { get; set; }
            public string SortField { get; set; }
            public string OppositeSortOrder { get; set; }
            public int Page { get; set; }
            public int TotalPages { get; set; }

        public class CustomerViewModel
        {
            public int CustomerId { get; set; }
            public string FullName{ get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string PersonalNumber { get; set; }
        }
    }
}
