using LeisureReviews.Models.Database;
using LeisureReviews.Models;
using Microsoft.AspNetCore.Mvc;
using LeisureReviews.Repositories.Interfaces;

namespace LeisureReviews.Controllers
{
    public class BaseController : Controller
    {
        protected IUsersRepository usersRepository { get; init; }

        protected async Task configureBaseModel(BaseViewModel model)
        {
            model.IsAuthorized = HttpContext.User.Identity.IsAuthenticated;
            model.CurrentUser = await getCurrentUser();
        }

        protected async Task<User> getCurrentUser() =>
                    await usersRepository.GetAsync(HttpContext.User);
    }
}
