using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopOnline.Data;
using ShopOnline.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopOnline.Controllers {
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly DataContext _db;

        public RoleController(RoleManager<IdentityRole> roleManager, DataContext db, UserManager<User> userManager) {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> Index() {
            var roles = await _db.Roles.AsNoTracking().ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(string roleName) {
            await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(string id) {
            var role = _db.Roles.Find(id);
            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(string id) {
            var role = await _roleManager.FindByIdAsync(id);
            await _roleManager.DeleteAsync(role);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Assign() => View(FillList());

        [HttpPost]
        public async Task<IActionResult> Assign(UserRoleDto userRoleDto) {
            User user = await _userManager.FindByIdAsync(userRoleDto.UserId);
            IdentityRole role = await _roleManager.FindByIdAsync(userRoleDto.RoleId);
            if (user == null || role == null) {
                return View(FillList());
            }
            var uerRole = _db.UserRoles.FirstOrDefault(U => U.UserId == userRoleDto.UserId);
            if (uerRole != null) {
                ViewBag.haveRole = "This user already have role";
                return View(FillList());
            }
            await _userManager.AddToRoleAsync(user, role.Name);
            return RedirectToAction(nameof(UserRole));
        }

        public ActionResult UserRole() {
            var result = from ur in _db.UserRoles
                         join r in _db.Roles on ur.RoleId equals r.Id
                         join a in _db.Users on ur.UserId equals a.Id
                         select new UserRoleMapingDto() {
                             UserId = ur.UserId,
                             RoleId = ur.RoleId,
                             UserEmail = a.Email,
                             RoleName = r.Name
                         };

            /*
                SELECT ur.UserId UserId, ur.RoleId RoleId, a.Email Email, r.Name RoleName
                FROM UserRoles ur
                INNER JOIN Users a ON a.Id = ur.userId
                INNER JOIN Roles r ON r.Id = ur.roleId
             */

            ViewBag.UserRoles = result;
            return View();
        }

        public UserRoleDto FillList() {
            List<IdentityRole> roles = _db.Roles.ToList();
            List<User> users = _db.Users.ToList();
            UserRoleDto userRole = new(roles, users);
            return userRole;
        }
    }
}
