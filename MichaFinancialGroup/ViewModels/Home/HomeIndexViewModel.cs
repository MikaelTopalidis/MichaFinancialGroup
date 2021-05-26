using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.ViewModels
{
    public class HomeIndexViewModel
    {

        public List<StatisticsPerCountryViewModel> Statistics { get; set; } = new List<StatisticsPerCountryViewModel>();
        public decimal TotalBalance { get; set; }
        public int Customers { get; set; }
        public int Accounts { get; set; }
        public decimal LargestAccount { get; set; }

        public class StatisticsPerCountryViewModel
        {
            public string Country { get; set; }
            public int Customers { get; set; }
            public int Accounts { get; set; }
            public decimal TotalBalance { get; set; }

        }
    }
}
