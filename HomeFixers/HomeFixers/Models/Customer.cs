using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeFixers.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [StringLength(50)]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }
        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required, StringLength(10)]
        [CustomValidation(typeof(Customer), "ValidateMobile")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [EmailAddress]
        public string EmailId { get; set; }
        [Required, StringLength(50)]
        [Display(Name = "Address Line 1")]
        public string Address_1 { get; set; }
        [StringLength(50)]
        [Display(Name = "Address Line 2")]
        public string Address_2 { get; set; }
        [Required, StringLength(20)]
        public string City { get; set; }
        [Required, StringLength(10)]
        public string State { get; set; }
        [Required, StringLength(10)]
        [CustomValidation(typeof(Customer), "ValidateZip")]
        public string Zip { get; set; }

        public virtual List<BucketList> bucketlist { get; set; }

        public virtual List<CustomerBankAccount> Card { get; set; }

        public static ValidationResult ValidateMobile(string mobile, ValidationContext context)
        {
            long num;
            bool isnum = long.TryParse(mobile, out num);
            if (!isnum)
            {
                return new ValidationResult("Mobile number must be a Number.");
            }
            else
            {
                if (mobile.Length < 10 || mobile.Length > 10)
                {
                    return new ValidationResult("Mobile number must have 10 digits.");
                }
                else
                    return ValidationResult.Success;
            }
        }

        public static ValidationResult ValidateZip(string zip, ValidationContext context)
        {
            int num;
            bool isnum = int.TryParse(zip, out num);
            if (!isnum)
            {
                return new ValidationResult("ZIP must be a Number.");
            }
            else
            {
                if (zip.Length < 5 || zip.Length > 5)
                {
                    return new ValidationResult("ZIP must be of 5 digits.");
                }
                else
                    return ValidationResult.Success;
            }
        }
    }
}