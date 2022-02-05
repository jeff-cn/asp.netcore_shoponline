using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Models;
using ShopOnline.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace ShopOnline.Controllers {
    [AllowAnonymous]
    public class HomeController : Controller {

        private readonly DataContext _db;
        private readonly UserManager<User> _userManager;

        public HomeController(DataContext db, UserManager<User> userManager) {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> Index() {
            var userName = User.Identity.Name;
            ViewData["LastProducts"] = _db.Products.Where(p => p.Available == true);

            if (userName is null)
                return View(_db.ProductsTypes.ToList());

            var user = await _userManager.FindByNameAsync(userName);
            var role = _userManager.GetRolesAsync(user).Result;
            if(role.Count == 0)
                return View(_db.ProductsTypes.ToList());

            HttpContext.Session.SetString("role", role[0]);
            HttpContext.Session.SetString("userId", user.Id);
            return View(_db.ProductsTypes.ToList());
        }


        public IActionResult IndexWhere(string ProductType, int? page) {
            ViewData["Type"] = ProductType;
            int count = _db.Products.Where(p => p.ProductTypes.Name == ProductType).Count();
            return View(Tuple.Create(_db.Products.Include(p => p.ProductTypes).Include(t => t.TagNames).Where(p => p.ProductTypes.Name == ProductType).ToList().ToPagedList(page ?? 1, 9), count));
        }

        public IActionResult MoreInfo(int? id) {
            FillList();
            if (id == null) {
                return NotFound();
            }
            var product = _db.Products.Find(id);
            if (product == null) {
                return NotFound();
            }
            return View(product);
        }

        public IActionResult Privacy() =>View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AddToCart(int? id) {
            List<Product> products = new List<Product>();
            if (id == null)
                return NotFound();

            var product = _db.Products.Include(p => p.ProductTypes).Include(t => t.TagNames).FirstOrDefault(c => c.Id == id);
            if (product == null)
                return NotFound();

            products = HttpContext.Session.Get<List<Product>>("products");
            if (products == null) 
                products = new List<Product>();

            products.Add(product);
            HttpContext.Session.Set("products", products);
            return RedirectToAction(nameof(Index));
        }

        public ActionResult RemoveFromCart(int? id) {
            List<Product> products = HttpContext.Session.Get<List<Product>>("products");
            var product = products.FirstOrDefault(p => p.Id == id);
            products.Remove(product);
            HttpContext.Session.Set("products", products);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Cart() {
            List<Product> products = HttpContext.Session.Get<List<Product>>("products");
            return View(products);
        }

        public ActionResult Remove(int? id) {
            List<Product> products = HttpContext.Session.Get<List<Product>>("products");
            var product = products.FirstOrDefault(p => p.Id == id);
            products.Remove(product);
            HttpContext.Session.Set("products", products);
            return RedirectToAction(nameof(Cart));
        }

        public void FillList() {
            ViewData["ProductTypeId"] = new SelectList(_db.ProductsTypes.ToList(), "Id", "Name");
            ViewData["TagNameId"] = new SelectList(_db.TagNames.ToList(), "Id", "Name");
        }
    }
}
