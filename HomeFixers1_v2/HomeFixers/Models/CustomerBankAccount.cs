using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeFixers.Models
{
    public class CustomerBankAccount
    {
        public int ID { get; set; }
        [Required,StringLength(50)]
        [Display(Name ="Name on Card")]
        public string NameOnCard { get; set; }
        [Required]
        [CustomValidation(typeof(CustomerBankAccount), "ValidateCard")]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }
        [Required]
        [CustomValidation(typeof(CustomerBankAccount), "ValidateCvv")]
        public string CVV { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }
        public string UserName { get; set; }

        public virtual int CustomerID { get; set; }
        public virtual Customer customer { get; set; }
        public virtual List<Transaction> transaction { get; set; }

        public static ValidationResult ValidateCard(string card, ValidationContext context)
        {
            long num;
            bool isnum = long.TryParse(card, out num);
            if (!isnum)
            {
                return new ValidationResult("Card number must be a Number.");
            }
            else
            {
                if (card.Length < 10 || card.Length > 10)
                {
                    return new ValidationResult("Card number must have 10 digits.");
                }
                else
                    return ValidationResult.Success;
            }
        }

        public static ValidationResult ValidateCvv(string cvv, ValidationContext context)
        {
            int num;
            bool isnum = int.TryParse(cvv, out num);
            if (!isnum)
            {
                return new ValidationResult("CVV must be a Number.");
            }
            else
            {
                if (cvv.Length < 3 || cvv.Length > 3)
                {
                    return new ValidationResult("CVV number must have 3 digits.");
                }
                else
                    return ValidationResult.Success;
            }
        }
    }
}