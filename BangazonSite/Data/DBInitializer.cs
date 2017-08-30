using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BangazonSite.Models;

namespace BangazonSite.Data //Worked on by Adam, August 30th, 2017
{
    public static class DBInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<ApplicationDbContext>();
            {
                if (context.ProductType.Any())
                {
                  return; //db is already seeded
                }

                var ProductType = new ProductType[]
                {
                    new ProductType()
                    {
                        Label = "Apparel"
                    },
                    new ProductType()
                    {
                        Label = "Sporting Goods"
                    },
                    new ProductType()
                    {
                        Label = "Books"
                    },
                    new ProductType()
                    {
                        Label = "Electronics"
                    },
                    new ProductType()
                    {
                        Label = "Handmade"
                    },
                    new ProductType()
                    {
                        Label = "Home and Garden"
                    },
                    new ProductType()
                    {
                        Label = "Jewelry"
                    },
                    new ProductType()
                    {
                        Label = "Music"
                    },
                    new ProductType()
                    {
                        Label = "Musical Instruments"
                    },
                    new ProductType()
                    {
                        Label = "Collectibles"
                    },
                    new ProductType()
                    {
                        Label = "Software and Computer Games"
                    }
                };
                foreach (ProductType pt in ProductType)
                {
                    context.ProductType.Add(pt);
                }
                context.SaveChanges();
            }
        }
    }
}