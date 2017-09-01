using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using BangazonSite.Data;

namespace BangazonSite.Models.ProductViewModels
{
    public class ProductCreateViewModel
    {
        public List<SelectListItem> ProductTypes { get; set; }
        public Product Product { get; set; }
        public ApplicationUser User { get; internal set; }
        public ProductCreateViewModel() { }

        public ProductCreateViewModel(ApplicationDbContext ctx)
        {
            // Creating SelectListItems will be used in a @Html.DropDownList
            // control in a Razor template. See Views/Products/Create.cshtml
            // for an example.
            this.ProductTypes = ctx.ProductType
                                    .OrderBy(l => l.Label)
                                    .AsEnumerable()
                                    .Select(li => new SelectListItem
                                    {
                                        Text = li.Label,
                                        Value = li.ProductTypeId.ToString()
                                    }).ToList();

            this.ProductTypes.Insert(0, new SelectListItem
            {
                Text = "Choose category...",
                Value = "0"
            });
        }
    }
}