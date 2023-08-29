using LeisureReviews.Models;
using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeisureReviews.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IUsersRepository usersRepository;

        private readonly IReviewsRepository reviewsRepository;

        public HomeController(IUsersRepository usersRepository, IReviewsRepository reviewsRepository)
        {
            this.usersRepository = usersRepository;
            this.reviewsRepository = reviewsRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var model = new BaseViewModel();
            await configureBaseModel(model);
            return View(model);
        }

        [HttpGet("Profile/{userName}")]
        public async Task<IActionResult> Profile(string userName)
        {
            var model = new ProfileViewModel();
            await configureBaseModel(model);
            model.User = await usersRepository.FindUserAsync(userName);
            if (model.User is null) return NotFound();
            model.Reviews = await reviewsRepository.GetAll(model.User.Id);
            return View(model);
        }

        [Authorize]
        [HttpGet("NewReview")]
        public async Task<IActionResult> NewReview()
        {
            var currentUser = await usersRepository.GetUserAsync(HttpContext.User);
            var model = new ReviewEditorViewModel { AuthorName = currentUser.UserName };
            await configureBaseModel(model);
            model.Review = new Review { AuthorId = currentUser.Id };
            return View("ReviewEditor", model);
        }

        [Authorize]
        [HttpPost("SaveReview")]
        public IActionResult SaveReview(Review review)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            reviewsRepository.SaveReview(review);
            return Ok(review);
        }

        private async Task configureBaseModel(BaseViewModel model)
        {
            model.IsAuthorized = HttpContext.User.Identity.IsAuthenticated;
            model.CurrentUser = await usersRepository.GetUserAsync(HttpContext.User);
        }
    }
}