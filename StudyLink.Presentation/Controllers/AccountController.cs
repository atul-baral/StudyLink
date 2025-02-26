using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace StudyLink.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return await RedirectBasedOnRole();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        return await RedirectBasedOnRole();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email or password is incorrect.");
                        return View(model);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task<IActionResult> RedirectBasedOnRole()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else if (roles.Contains("Teacher"))
            {
                return RedirectToAction("Index", "Home", new { area = "Teacher" });
            }
            else if (roles.Contains("Student"))
            {
                return RedirectToAction("Index", "Home", new { area = "Student" });
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
