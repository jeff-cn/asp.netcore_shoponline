using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.Data;
using ShopOnline.Email;
using ShopOnline.Models;
using System;
using System.Threading.Tasks;

namespace ShopOnline.Controllers {
    [AllowAnonymous]
    public class AccountController : Controller {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DataContext _db;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, DataContext db) {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        [HttpGet]
        public ActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(UserDto userDto) {
            if (!ModelState.IsValid)
                return View();

            User user = await _userManager.FindByEmailAsync(userDto.Email); //_db.Users.FirstOrDefault(u => u.Email == userDto.Email);

            if (user is not null) {
                ViewData["exist"] = "This email is already exist";
                return View(userDto);
            }

            user = new() {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Name = userDto.FirstName + " " + userDto.LastName,
                UserName = Guid.NewGuid().ToString(),
                Email = userDto.Email,
                BirthDay = userDto.BirthDay,
                PhoneNumber = userDto.Phone
            };

            IdentityResult succeeded = await _userManager.CreateAsync(user, userDto.Password);
            if (!succeeded.Succeeded)
                return View();

            /*var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
            EmailHelper emailHelper = new EmailHelper();
            bool emailResponse = emailHelper.SendEmail(user.Email.Trim(), confirmationLink);

            if (!emailResponse)
                return View();*/

            await _signInManager.SignInAsync(user, true);
            await _userManager.AddToRoleAsync(user, "user");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(UserDto userDto, string returnUrl) {
            User user = await _userManager.FindByEmailAsync(userDto.Email);
            bool isHe = await _userManager.CheckPasswordAsync(user, userDto.Password);

            if (!isHe) {
                ViewBag.isHe = "Email or Password is invalid";
                return View();
            }

            /*bool emailStatus = await _userManager.IsEmailConfirmedAsync(user);
            if (emailStatus == false) {
                ModelState.AddModelError("", "Email is unconfirmed, please confirm it first");
                return View();
            }*/

            await _signInManager.SignInAsync(user, true);
            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout() {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email) {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return View("Error");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            await _signInManager.SignInAsync(user, true);
            return View(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }
    }
}
