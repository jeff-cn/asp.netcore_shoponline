using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ShopOnline.Controllers {
    [Authorize(Roles = "Admin")]
    public class ProductTypeController : Controller {
        private readonly DataContext _db;

        public ProductTypeController(DataContext db) {
            _db = db;
        }

        public IActionResult Index() => View(_db.ProductsTypes.ToList());

        public ActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductType productType) {
            if (ModelState.IsValid) {
                if (IfExiste(productType)) {
                    ViewBag.message = "This product is already exist";
                    return View(productType);
                }
                _db.ProductsTypes.Add(productType);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productType);
        }

        public ActionResult Edit(int? id) {
            if (id == null) {
                return NotFound();
            }
            var productType = _db.ProductsTypes.Find(id);
            if (productType == null) {
                return NotFound();
            }
            return View(productType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductType productType) {
            if (ModelState.IsValid) {
                if (IfExiste(productType)) {
                    ViewBag.message = "This product is already exist";
                    return View(productType);
                }
                _db.Update(productType);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productType);
        }

        [HttpGet]
        public IActionResult Delete(int? id) {
            if (id == null) {
                return NotFound();
            }
            var productType = _db.ProductsTypes.Find(id);
            if (productType == null) {
                return NotFound();
            }
            return View(productType);
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int id) {
            var productType = _db.ProductsTypes.Find(id);
            _db.Remove(productType);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IfExiste(ProductType productType) {
            var searchProductType = _db.ProductsTypes.FirstOrDefault(p => p.Name == productType.Name && p.Id != productType.Id);
            if (searchProductType != null) {
                return true;
            }
            return false;
        }
    }
}
