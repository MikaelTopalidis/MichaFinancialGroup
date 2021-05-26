using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.ViewModels.Customer
{
    public class CustomerNewViewModel
    {
        public List<SelectListItem> Genders { get; set; }
        [Required]
        public string SelectedGender { get; set; }
        [Required]
        [MaxLength(100)]
        public string Givenname { get; set; }
        [Required]
        [MaxLength(100)]
        public string Surname { get; set; }
        [Required]
        [MaxLength(100)]
        public string Streetaddress { get; set; }
        [Required]
        [MaxLength(100)]
        public string City { get; set; }
        [Required]
        [MaxLength(15)]
        public string Zipcode { get; set; }
        public List<SelectListItem> Countries { get; set; }
        [Required]
        public string SelectedCountry { get; set; }
        [MaxLength(3)]
        public string CountryCode { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        [Required]
        [MaxLength(20)]
        public string NationalId { get; set; }
        [Required]
        [MaxLength(10)]
        public string Telephonecountrycode { get; set; }
        [Required]
        [MaxLength(25)]
        public string Telephonenumber { get; set; }
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Emailaddress { get; set; }
    }
}
