using LeisureReviews.Models;
using LeisureReviews.Repositories.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeisureReviews.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IUsersRepository usersRepository;

        public HomeController(IUsersRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        public IActionResult Index()
        {
            var model = new BaseViewModel();
            configureBaseModel(model);
            return View(model);
        }

        [HttpGet("Profile/{username}")]
        public async Task<IActionResult> Profile(string userName)
        {
            var model = new ProfileViewModel();
            configureBaseModel(model);
            model.User = await usersRepository.FindUserAsync(userName);
            return View(model);
        }

        private void configureBaseModel(BaseViewModel model)
        {
            model.IsAuthorized = HttpContext.User.Identity.IsAuthenticated;
            model.UserName = HttpContext.User.Identity.Name;
        }
    }
}