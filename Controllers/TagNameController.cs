using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ShopOnline.Controllers {
    [Authorize(Roles = "Admin")]
    public class TagNameController : Controller {
        private readonly DataContext _db;

        public TagNameController(DataContext db) {
            _db = db;
        }

        public async Task<IActionResult> Index() {
            return View(await _db.TagNames.ToListAsync());
        }

        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TagName tagName) {
            if (ModelState.IsValid) {
                if (ifExiste(tagName)) {
                    ViewBag.message = "This product is already exist";
                    return View(tagName);
                }
                _db.Add(tagName);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tagName);
        }

        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var tagNames = await _db.TagNames.FindAsync(id);
            if (tagNames == null) {
                return NotFound();
            }
            return View(tagNames);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TagName tagName) {
            if (id != tagName.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    if (ifExiste(tagName)) {
                        ViewBag.message = "This product is already exist";
                        return View(tagName);
                    }
                    _db.Update(tagName);
                    await _db.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!TagNamesExists(tagName.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tagName);
        }

        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var tagNames = await _db.TagNames
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tagNames == null) {
                return NotFound();
            }

            return View(tagNames);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var tagNames = await _db.TagNames.FindAsync(id);
            _db.TagNames.Remove(tagNames);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TagNamesExists(int id) {
            return _db.TagNames.Any(e => e.Id == id);
        }
        private bool ifExiste(TagName tagName) {
            var search = _db.TagNames.FirstOrDefault(p => p.Name == tagName.Name && p.Id != tagName.Id);
            if (search != null) {
                return true;
            }
            return false;
        }
    }
}
