using BangazonSite.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonSite.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public virtual ApplicationUser User { get; set; }

        public int? PaymentTypeId { get; set; }

        [Display(Name = "PaymentType")]
        public PaymentType PaymentType { get; set; }

       
        public DateTime DateCreated { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateCompleted { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

    }
}
