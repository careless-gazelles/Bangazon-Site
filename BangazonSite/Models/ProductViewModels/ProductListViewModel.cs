using System.Collections.Generic;
using BangazonSite.Models;
using BangazonSite.Data;

namespace BangazonSite.Models.ProductViewModels
{
    public class ProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
    }
}