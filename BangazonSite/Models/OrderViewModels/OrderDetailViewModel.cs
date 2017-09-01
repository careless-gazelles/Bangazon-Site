using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonSite.Models.OrderViewModels
{
    public class OrderDetailViewModel
    {
        public List<Product> Products { get; set; }
        public Order Order { get; set; }
    }
}
