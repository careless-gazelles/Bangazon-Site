using System.Collections.Generic;
using BangazonSite.Models;
using BangazonSite.Data;

namespace BangazonSite.Models.ProductViewModels
{
    public class ProductTypesViewModel
    {
        public IEnumerable<ProductType> ProductTypes { get; set; }
    }
}