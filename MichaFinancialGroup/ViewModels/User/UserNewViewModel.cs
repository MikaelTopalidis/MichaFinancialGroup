using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.ViewModels.User
{
    public class UserNewViewModel
    {
        public int Id { get; set; }
        [Required]
        public List<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>();
        [Required]
        public string SelectedRoleId { get; set; }

        [Required]
        [MaxLength(50)]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^((?=.*[a-z])(?=.*[A-Z])(?=.*\\d)).+$", ErrorMessage = "Password must contain atleast 1 of each; Uppercase, number and a symbol")]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Password doesn´t match")]
        public string ConfirmPassword { get; set; }
    }
}
