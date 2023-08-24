﻿using LeisureReviews.Models;
using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.interfaces;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeisureReviews.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsersRepository usersRepository;

        private readonly SignInManager<User> signInManager;

        public AccountController(IUsersRepository usersRepository, SignInManager<User> signInManager)
        {
            this.usersRepository = usersRepository;
            this.signInManager = signInManager;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult AdditionalInfo(string email)
        {
            AdditionalInfoModel model = new AdditionalInfoModel { Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginModel model, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                    return redirectTo(returnUrl);
                else
                    ModelState.AddModelError(string.Empty, "Incorrect username or password.");
            }
            return View(model);
        }

        public IActionResult GoogleSignIn()
        {
            string redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);
            return new ChallengeResult(GoogleDefaults.AuthenticationScheme, properties);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null) RedirectToAction("SignIn");
            return await checkInfoAsync(info);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAdditionalInfo(AdditionalInfoModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Username, Email = model.Email };
                var result = await usersRepository.CreateAsync(user);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                    addModelErrors(result.Errors);
            }
            return View("AdditionalInfo", model);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(LoginModel model, string returnUrl = "")
        {
            IActionResult result = View();
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Username };
                await registerUserAsync(user, model.Password, onSuccessAsync: async () =>
                {
                    await signInManager.SignInAsync(user, model.RememberMe);
                    result = redirectTo(returnUrl);
                });
            }
            return result;
        }

        public async new Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task<IActionResult> checkInfoAsync(ExternalLoginInfo info)
        {
            var user = await usersRepository.FindUserAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
            if (user is null)
                return RedirectToAction("AdditionalInfo", new { email = info.Principal.FindFirstValue(ClaimTypes.Email) });
            await signInManager.SignInAsync(user, true);
            return RedirectToAction("Index", "Home");
        }

        private async Task registerUserAsync(User user, string password, Func<Task> onSuccessAsync)
        {
            var result = await usersRepository.CreateAsync(user, password);
            if (result.Succeeded)
                await onSuccessAsync.Invoke();
            else addModelErrors(result.Errors);
        }

        private void addModelErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
                ModelState.AddModelError(String.Empty, error.Description);
        }

        private IActionResult redirectTo(string url)
        {
            if (!String.IsNullOrEmpty(url) && Url.IsLocalUrl(url))
                return Redirect(url);
            return RedirectToAction("Index", "Home");
        }
    }
}