using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.ViewModels
{
    public class CustomerDetailsViewModel
    {
        public decimal TotalBalance { get; set; }
        public int CustomerId { get; set; }
        public string Gender { get; set; }
        public string Givenname { get; set; }
        public string Surname { get; set; }
        public string Streetaddress { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public DateTime Birthday { get; set; }
        public string NationalId { get; set; }
        public string Telephonecountrycode { get; set; }
        public string Telephonenumber { get; set; }
        public string Emailaddress { get; set; }
        public List<DispositionsViewModel> Dispositions { get; set; }
        public class DispositionsViewModel
        {
            public AccountsViewModel Account { get; set; }

        }

        public class AccountsViewModel
        {
            public int AccountId { get; set; }
            public string Type { get; set; }
            public string Frequency { get; set; }
            public DateTime Created { get; set; }
            public decimal Balance { get; set; }
        }
    }


}
