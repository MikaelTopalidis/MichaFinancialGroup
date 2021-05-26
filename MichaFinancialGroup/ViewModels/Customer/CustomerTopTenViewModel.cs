using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.ViewModels.Customer
{
    public class CustomerTopTenViewModel
    {
        public string Country { get; set; }
        public List<TopTenViewModel> Customers { get; set; } = new List<TopTenViewModel>();
        public class TopTenViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal TotalBalance { get; set; }
        }
    }

}
