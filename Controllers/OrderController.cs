using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopOnline.Controllers {
    public class OrderController : Controller {

        private readonly DataContext _db;
        private readonly UserManager<User> _userManager;
        public OrderController(DataContext db, UserManager<User> userManager) {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Checkout() {
            string userid = HttpContext.Session.GetString("userId");
            User user = await _userManager.FindByIdAsync(userid);
            ViewData["userEmail"] = user.Email;
            ViewData["userPhone"] = user.PhoneNumber;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order) {
            List<Product> products = HttpContext.Session.Get<List<Product>>("products");
            string userid = HttpContext.Session.GetString("userId");
            var GUID = Guid.NewGuid();
            order.Guid = GUID;
            order.DateTime = DateTime.Now;
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            Order MyOrder = _db.Orders.FirstOrDefault(o => o.Guid == GUID);
            if (products != null) {
                foreach (var product in products) {
                    OrderUserProduct orderUserProduct = new() {
                        OrderId = MyOrder.Id,
                        ProductId = product.Id,
                        UserId = userid
                    };
                    _db.OrderUsers.Add(orderUserProduct);
                }
            }
            await _db.SaveChangesAsync();
            HttpContext.Session.Set("products", new List<Product>());
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Order() {
            return View(await _db.OrderUsers.Include(o => o.Order).Include(p => p.Product).Include(u => u.User).ToListAsync());
        }
    }
}
