using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HomeFixers.Models
{
    public class ServiceProvider
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
        [Required]
        [CustomValidation(typeof(ServiceProvider),"ValidateMobile")]
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
        [Required, StringLength(5)]
        [CustomValidation(typeof(ServiceProvider), "ValidateZip")]
        public string Zip { get; set; }
        [CustomValidation(typeof(ServiceProvider), "ValidateSSN")]
        public string SSN { get; set; }
        public bool Verified { get; set; }
        public virtual int ServiceID { get; set; }
        public virtual Service service { get; set; }

        public virtual List<BucketList> bucketlist { get; set; }

        public virtual List<Schedule> ScheduleList { get; set; }

        //public virtual List<ProviderBankAccount> Account { get; set; }

        public static ValidationResult ValidateMobile(string mobile,ValidationContext context)
        {
            long num;
            bool isnum = long.TryParse(mobile, out num);
            if (!isnum)
            {
                return new ValidationResult("Mobile number must be a Number.");
            }
            else
            {
                if(mobile.Length < 10 || mobile.Length > 10)
                {
                    return new ValidationResult("Mobile number must have 10 digits.");
                }
                else
                return ValidationResult.Success;
            }
        }

        public static ValidationResult ValidateSSN(string ssn, ValidationContext context)
        {
            long num;
            bool isnum = long.TryParse(ssn, out num);
            if (!isnum)
            {
                return new ValidationResult("SSN must be a Number.");
            }
            else
            {
                if (ssn.Length < 9 || ssn.Length > 9)
                {
                    return new ValidationResult("SSN must have 9 digits.");
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