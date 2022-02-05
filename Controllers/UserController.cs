using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using System;
using System.Threading.Tasks;

namespace ShopOnline.Controllers {
    [Authorize(Roles = "Admin")]
    public class UserController : Controller {
        private readonly DataContext _db;
        public UserController(DataContext db) {
            _db = db;
        }
        public async  Task<IActionResult> Index() {
            var users = await _db.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> ActiveUser(string id) {
            var user = _db.Users.Find(id);
            user.LockoutEnd = null;
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeactiveUser(string id) {
            var user = _db.Users.Find(id);
            user.LockoutEnd = DateTime.Now.AddYears(100);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteUser(string id) {
            var user = _db.Users.Find(id);
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
