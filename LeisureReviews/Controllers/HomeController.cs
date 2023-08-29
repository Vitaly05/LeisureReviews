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
            var user = await usersRepository.FindUserAsync(userName);
            if (user is null) return NotFound();
            var model = new ProfileViewModel
            {
                User = user,
                Reviews = await reviewsRepository.GetAll(user.Id)
            };
            await configureBaseModel(model);
            return View(model);
        }

        [Authorize]
        [HttpGet("NewReview")]
        public async Task<IActionResult> NewReview()
        {
            var model = new ReviewEditorViewModel();
            await configureBaseModel(model);
            model.AuthorName = model.CurrentUser.UserName;
            model.Review = new Review { AuthorId = model.CurrentUser.Id };
            return View("ReviewEditor", model);
        }

        [Authorize]
        [HttpGet("EditReview/{reviewId}")]
        public async Task<IActionResult> EditReview(string reviewId)
        {
            var model = new ReviewEditorViewModel();
            await configureBaseModel(model);
            model.Review = await reviewsRepository.Get(reviewId);
            model.AuthorName = model.CurrentUser.UserName;
            if (model.Review is null) return NotFound();
            if (model.CurrentUser.Id != model.Review.AuthorId) return Forbid();
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