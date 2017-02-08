using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeFixers.Models
{
    public class BucketList
    {
        public int ID { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Service date")]
        public DateTime DateAdded { get; set; }
        [Required]
        public string Description { get; set; }
        
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
        [CustomValidation(typeof(BucketList), "ValidateZip")]
        public string Zip { get; set; }
        public bool Completed { get; set; }
        public string slotbooked { get; set; }

        public virtual int CustomerID { get; set; }
        public virtual Customer customer { get; set; }

        public virtual int ServiceProviderID { get; set; }
        public virtual ServiceProvider serviceprovider { get; set; }

        

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