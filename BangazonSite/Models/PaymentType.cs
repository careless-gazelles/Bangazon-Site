using BangazonSite.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BangazonSite.Models
{
    public class PaymentType
    {
        [Key]
        public int PaymentTypeId { get; set; }

        public PaymentType() {
            IsActive = true;
        }

        public bool IsActive { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? DateCreated { get; set; }

        [Required]
        [StringLength(12)]
        public string Description { get; set; }

        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; }

        [Required]
        public virtual ApplicationUser User { get; set; }
    }
}