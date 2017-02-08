using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeFixers.Models
{
    public class ProviderBankAccount
    {
        public int ID { get; set; }
        [Required, StringLength(50)]
        public string AccountName { get; set; }
        [Required, MaxLength(10), MinLength(10)]
        [CustomValidation(typeof(ProviderBankAccount), "ValidateAccount")]
        public string AccountNumber { get; set; }
        [Required, MaxLength(5), MinLength(5)]
        [CustomValidation(typeof(ProviderBankAccount), "ValidateRouting")]
        public string RoutingNumber { get; set; }
        
        public bool Active { get; set; }

        //public virtual int ProviderID { get; set; }
        //public virtual ServiceProvider provider { get; set; }

        public static ValidationResult ValidateAccount(string account, ValidationContext context)
        {
            long num;
            bool isnum = long.TryParse(account, out num);
            if (!isnum)
            {
                return new ValidationResult("Account number must be a Number.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }

        public static ValidationResult ValidateRouting(string routing, ValidationContext context)
        {
            int num;
            bool isnum = int.TryParse(routing, out num);
            if (!isnum)
            {
                return new ValidationResult("Routing Number must be a Number.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}