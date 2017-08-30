using System.Collections.Generic;
using BangazonSite.Models;
using BangazonSite.Data;



namespace BangazonSite.Models.ProductViewModels
{
    public class ProductTypeDetailViewModel
    {
        public ProductType ProductType { get; set; }
        public List<Product> Products { get; set; }
    }
}