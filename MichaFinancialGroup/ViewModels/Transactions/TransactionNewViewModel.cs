using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MichaFinancialGroup.ViewModels
{


    public class TransactionNewViewModel : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (selectedOperation == "0")
            {
                yield return new ValidationResult("Please select an operation",
                    new[] { nameof(selectedOperation) });
            }

            if (selectedOperation != "Credit in Cash" && Amount > CurrentBalance)
                yield return new ValidationResult(
                    "Insufficient funds. \n Amount cannot be greater than Balance.",
                    new[] { nameof(Amount) });

            if (Amount < 1)
                yield return new ValidationResult(
                   "Minimum transaction in 1 USD",
                   new[] { nameof(Amount) });
 
            if ((selectedOperation == "Transfer to Another account") && (ToAccountId <= 0 || ToAccountId.ToString().Length < 1))
            {
                yield return new ValidationResult("Recieving account is required with specific transactiontype selected",
                    new[] { nameof(Account) });
            }

        }

        public int TransactionId { get; set; }

        [Required]
        public int AccountId { get; set; }

        public int ToAccountId { get; set; }
        public DateTime Date { get; set; }
        public List<SelectListItem> Operations { get; set; }

        [Required]
        public string selectedOperation { get; set; }

        [Required]
        public decimal Amount { get; set; }
        [Required]
        public decimal CurrentBalance { get; set; }
        public string Symbol { get; set; }
        public string Bank { get; set; }
        public string Account { get; set; }
    }
}
