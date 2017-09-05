using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BangazonSite.Data;
using BangazonSite.Models;
using Microsoft.AspNetCore.Identity;
using BangazonSite.Models.OrderViewModels;

namespace BangazonSite.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext ctx, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = ctx;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUserAsync();
            var applicationDbContext = _context.Order.Include(o => o.PaymentType).Where(o => o.User == currentUser);
            return View(await applicationDbContext.ToListAsync());
        }


        //KC- Get Open order per customerID
        //by determining in orders table if this customer has an order without paytype -- Paytype ==null 
        // bind this product to the orderId, placing this entire instance in the orderProduct Table as a line item.
        public async Task<IActionResult> AddProductToOrder(Product productToAdd)
        {
            var currentUser = await GetCurrentUserAsync();
            int? custOpenOrder = (from order in _context.Order
                                  where order.PaymentTypeId == null && order.User == currentUser
                                  select order.OrderId).SingleOrDefault();
            if (custOpenOrder > 0)
            {
                OrderProduct orderProduct = new OrderProduct() {
                    OrderId = (int)custOpenOrder,
                    ProductId = productToAdd.ProductId
                };
                //kc-getready to add to db
                _context.Add(orderProduct);
                //kc- actually add to db
               
            } if (custOpenOrder == 0)
            {
                Order newOrder = new Order
                {
                    DateCreated = DateTime.Now,
                    User = currentUser
                };
                _context.Add(newOrder);

                OrderProduct orderProduct = new OrderProduct()
                {
                    OrderId = newOrder.OrderId,
                    ProductId = productToAdd.ProductId
                };
                _context.Add(orderProduct);

            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            OrderDetailViewModel currentOrderModel = new OrderDetailViewModel();

            if (id == null)
            {
                return NotFound();
            }

            currentOrderModel.Order = await _context.Order
                .Include(o => o.PaymentType)
                .Include(op => op.OrderProducts)
                .SingleOrDefaultAsync(m => m.OrderId == id);
            if (currentOrderModel.Order == null)
            {
                return NotFound();
            }

            foreach (var orderProduct in currentOrderModel.Order.OrderProducts)
            {
                var currentProductsInOrder = _context.Product.SingleOrDefault(p => p.ProductId == orderProduct.ProductId);
                currentOrderModel.Products.Add(currentProductsInOrder);
            }

            return View(currentOrderModel);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType, "PaymentTypeId", "AccountNumber");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,PaymentTypeId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType, "PaymentTypeId", "AccountNumber", order.PaymentTypeId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            OrderDetailViewModel orderDetail = new OrderDetailViewModel();

            var order = await _context.Order
                .Include(o => o.PaymentType)
                .Include(o => o.OrderProducts)
                .SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            
            // Ollie - 9/1
            // Get the products that belong to each order
            orderDetail.Products = (
                from p in _context.Product
                join op in order.OrderProducts
                on p.ProductId equals op.ProductId
                where op.OrderId == id
                select p
                ).ToList();

            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType, "PaymentTypeId", "AccountNumber", order.PaymentTypeId);

            orderDetail.Order = order;
            return View(orderDetail);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,PaymentTypeId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            OrderDetailViewModel orderDetail = new OrderDetailViewModel();

            // Ollie - 9/1
            // Get the products that belong to each order
            orderDetail.Products = (
                from p in _context.Product
                join op in _context.OrderProduct
                on p.ProductId equals op.ProductId
                where op.OrderId == id
                select p
                ).ToList();


            // Ollie - 9/1 
            // Apparently the user gets added to the Product object before it's passed here
            // And the DateCreated was causing issues
            // This removes them, thus making the ModelState valid
            ModelState.Remove("order.User");
            ModelState.Remove("order.DateCreated");

            orderDetail.Order = order;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    foreach(Product product in orderDetail.Products)
                    {
                        product.Quantity -= 1;
                        _context.Update(product);
                        _context.SaveChanges();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType, "PaymentTypeId", "AccountNumber", order.PaymentTypeId);

            return View(orderDetail);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            OrderDetailViewModel orderDetail = new OrderDetailViewModel();

            var order = await _context.Order
                .Include(o => o.PaymentType)
                .Include(o => o.OrderProducts)
                .SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            orderDetail.Order = order;

            // Ollie - 9/1
            // Get the products that belong to each order
            orderDetail.Products = (
                from p in _context.Product
                join op in order.OrderProducts
                on p.ProductId equals op.ProductId
                where op.OrderId == id
                select p
                ).ToList();

            return View(orderDetail);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.SingleOrDefaultAsync(m => m.OrderId == id);

            List<OrderProduct> orderProducts = await _context.OrderProduct.Where(x => x.OrderId == id).ToListAsync();

            foreach (var op in orderProducts)
            {
                _context.OrderProduct.Remove(op);
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }
    }
}
