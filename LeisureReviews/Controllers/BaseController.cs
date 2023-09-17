using LeisureReviews.Models.Database;
using Microsoft.AspNetCore.Mvc;
using LeisureReviews.Repositories.Interfaces;
using LeisureReviews.Models.ViewModels;

namespace LeisureReviews.Controllers
{
    public class BaseController : Controller
    {
        protected IUsersRepository usersRepository { get; init; }

        public BaseController(IUsersRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        protected async Task configureBaseModelAsync(BaseViewModel model)
        {
            model.IsAuthorized = HttpContext.User.Identity.IsAuthenticated;
            model.CurrentUser = await getCurrentUserAsync();
            if (model.CurrentUser is not null)
                model.CurrentUser.Roles = await usersRepository.GetRolesAsync(model.CurrentUser);
        }

        protected async Task<User> getCurrentUserAsync() =>
                    await usersRepository.GetAsync(HttpContext.User);
    }
}
