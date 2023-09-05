using LeisureReviews.Models;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LeisureReviews.Controllers
{
    [Route("")]
    public class HomeController : BaseController
    {
        private readonly IReviewsRepository reviewsRepository;

        public HomeController(IUsersRepository usersRepository, IReviewsRepository reviewsRepository)
        {
            this.usersRepository = usersRepository;
            this.reviewsRepository = reviewsRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 0, int pageSize = 5)
        {
            var model = new HomeViewModel()
            {
                Page = page,
                PageSize = pageSize,
                PagesCount = await reviewsRepository.GetPagesCountAsync(pageSize),
                Reviews = await reviewsRepository.GetLatestAsync(page, pageSize)
            };
            await configureBaseModel(model);
            return View(model);
        }

        [HttpGet("Profile/{userName}")]
        public async Task<IActionResult> Profile(string userName)
        {
            var user = await usersRepository.FindAsync(userName);
            if (user is null) return NotFound();
            var model = new ProfileViewModel
            {
                User = user,
                Reviews = await reviewsRepository.GetAllAsync(user.Id)
            };
            await configureBaseModel(model);
            return View(model);
        }
    }
}