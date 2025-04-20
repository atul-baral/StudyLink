using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyLink.Application.Services.Interface;
using StudyLink.Application.ViewModels;
using StudyLink.Domain.Entities;
using System;
using System.Net;
using System.Threading.Tasks;

namespace StudyLink.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
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



        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["Success"] = "If this email is registered, a reset link has been sent.";
                return RedirectToAction("Login");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new
            {
                email = user.Email,
                token = WebUtility.UrlEncode(token)
            }, protocol: Request.Scheme);

            string body = $"Click the link to reset your password: <a href='{callbackUrl}'>Reset Password</a>";
            await _emailService.SendEmailAsync(user.Email, "Reset Password", body);

            TempData["Success"] = "Reset link sent to your email.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordVM
            {
                Email = email,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Token = WebUtility.UrlDecode(model.Token); 

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["Error"] = "Invalid user.";
                return RedirectToAction("Login");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Password has been reset. You can now log in.";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return RedirectToAction("Login");
        }


    }
}
