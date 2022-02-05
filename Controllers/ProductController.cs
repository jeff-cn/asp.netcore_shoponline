using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace ShopOnline.Controllers {
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller {
        private readonly DataContext _db;
        public readonly IWebHostEnvironment _he;

        public ProductController(DataContext db, IWebHostEnvironment he) {
            _db = db;
            _he = he;
        }

        public IActionResult Index(int? page) => View(Tuple.Create(_db.Products.Include(p => p.ProductTypes).Include(t => t.TagNames).ToList().ToPagedList(page ?? 1, 9), _db.Products.Count()));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string name)
        {
            if (name == null || name == "")
                return RedirectToAction(nameof(Index));

            return View(_db.Products.Include(p => p.ProductTypes).Include(t => t.TagNames).Where(p => p.Name.Contains(name)).ToList());
        }

        public IActionResult Create() {
            FillList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile image) {
            if (!ModelState.IsValid)
                return View(product);
            
            if (IfExiste(product)) {
                ViewBag.message = "This product is already exist";
                FillList();
                return View(product);
            }

            if (image != null) {
                var name = Guid.NewGuid().ToString() + image.FileName;
                var path = Path.Combine(_he.WebRootPath + "/images", Path.GetFileName(name));
                await image.CopyToAsync(new FileStream(path, FileMode.Create));
                product.Image = name;
            } else {
                product.Image = "no-image.png";
            }

            product.Time = DateTime.Now;
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id) {
            FillList();
            var product = _db.Products.Find(id);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile image) {
            if (!ModelState.IsValid) 
                return View(product);

            if (IfExiste(product)) {
                ViewBag.message = "This product is already exist";
                FillList();
                return View(product);
            }

            product.Time = DateTime.Now;
            Product oldProduct = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == product.Id);

            if (image != null) {

                if (oldProduct.Image != "no-image.png") {
                    DeletImageFromwwwroot(oldProduct);
                }

                var newNameImage = Guid.NewGuid().ToString() + image.FileName;
                var newFullPath = Path.Combine(_he.WebRootPath + "/images", newNameImage);
                await image.CopyToAsync(new FileStream(newFullPath, FileMode.Create));

                product.Image = newNameImage;

                _db.Products.Update(product);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            } else {
                _db.Products.Update(product);
                _db.Entry(product).Property(p => p.Image).IsModified = false;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult Details(int? id) {
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

        public ActionResult Delete(int? id) {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Product product) {
            Product oldProduct = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == product.Id);

            if (product.Image != "no-image.png") {
                DeletImageFromwwwroot(oldProduct);
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteImage(int id) {
            Product product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            if (product.Image == "no-image.png") 
                return RedirectToAction(nameof(Index));

            DeletImageFromwwwroot(product);

            product.Image = "no-image.png";

            _db.Products.Update(product);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public void FillList() {
            ViewBag.ProductTypeId = new SelectList(_db.ProductsTypes.ToList(), "Id", "Name");
            ViewBag.TagNameId = new SelectList(_db.TagNames.ToList(), "Id", "Name");
        }

        private bool IfExiste(Product product) {
            var searchProductType = _db.Products.FirstOrDefault(p => p.Name == product.Name && p.Id != product.Id);
            if (searchProductType != null) {
                return true;
            }
            return false;
        }

        public void DeletImageFromwwwroot(Product product) {
            var oldNameImage = product.Image;
            var oldFullPath = Path.Combine(_he.WebRootPath + "/images", oldNameImage);
            System.IO.File.Delete(oldFullPath);
        }
    }
}
