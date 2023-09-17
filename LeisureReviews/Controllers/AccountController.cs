using LeisureReviews.Data;
using LeisureReviews.Models;
using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LeisureReviews.Controllers
{
    public class AccountController : BaseController
    {
        private readonly SignInManager<User> signInManager;

        private readonly UserManager<User> userManager;

        public AccountController(IUsersRepository usersRepository, SignInManager<User> signInManager, UserManager<User> userManager) : base(usersRepository)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult AdditionalInfo(string externalProvider, string providerKey)
        {
            var model = new AdditionalInfoModel { ExternalProvider = externalProvider, ProviderKey = providerKey };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginModel model, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                if (await signInAsync(model))
                    return redirectTo(returnUrl);
            }
            return View(model);
        }

        public IActionResult ExternalSignIn(string scheme)
        {
            string redirectUrl = Url.Action("ExternalSignInResponse", "Account");
            var properties = signInManager.ConfigureExternalAuthenticationProperties(scheme, redirectUrl);
            return new ChallengeResult(scheme, properties);
        }

        public async Task<IActionResult> ExternalSignInResponse()
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
                var user = new User { UserName = model.Username, ExternalProvider = model.ExternalProvider, ProviderKey = model.ProviderKey };
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeStatusModel model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var user = await usersRepository.FindAsync(model.UserName);
            if (user is null) return BadRequest();
            await usersRepository.ChangeStatusAsync(user, model.Status);
            return Ok(model.Status.ToString());
        }

        private async Task<bool> signInAsync(LoginModel model)
        {
            var user = await usersRepository.FindAsync(model.Username);
            if (!await passwordIsValidAsync(user, model.Password) || !accountIsActive(user)) return false;
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
            if (!result.Succeeded) return false;
            return true;
        }

        private async Task<bool> passwordIsValidAsync(User user, string password)
        {
            if (!await userManager.CheckPasswordAsync(user, password))
            {
                ModelState.AddModelError(string.Empty, "Incorrect username or password.");
                return false;
            }
            return true;
        }

        private bool accountIsActive(User user)
        {
            if (user.Status == AccountStatus.Blocked)
            {
                ModelState.AddModelError(string.Empty, "Account was blocked.");
                return false;
            }
            return true;
        }

        private async Task<IActionResult> checkInfoAsync(ExternalLoginInfo info)
        {
            var user = await usersRepository.FindAsync(info.LoginProvider, info.ProviderKey);
            if (user is not null && !accountIsActive(user)) return View("SignIn");
            if (user is null)
                return RedirectToAction("AdditionalInfo", new { externalProvider = info.LoginProvider, providerKey = info.ProviderKey });
            await signInManager.SignInAsync(user, true);
            return RedirectToAction("Index", "Home");
        }

        private async Task registerUserAsync(User user, string password, Func<Task> onSuccessAsync)
        {
            var result = await usersRepository.CreateAsync(user, password);
            if (result.Succeeded) await onSuccessAsync.Invoke();
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
