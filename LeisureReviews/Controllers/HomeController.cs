using LeisureReviews.Models;
using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

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
        public IActionResult Index()
        {
            var model = new BaseViewModel();
            configureBaseModel(model);
            return View(model);
        }

        [HttpGet("Profile/{userName}")]
        public async Task<IActionResult> Profile(string userName)
        {
            var model = new ProfileViewModel();
            model.User = await usersRepository.FindUserAsync(userName);
            configureBaseModel(model);
            return View(model);
        }

        [Authorize]
        [HttpGet("NewReview")]
        public async Task<IActionResult> NewReview()
        {
            var currentUser = await usersRepository.GetUserAsync(HttpContext.User);
            var model = new ReviewEditorViewModel { AuthorName = currentUser.UserName };
            model.Review = new Review { AuthorId = currentUser.Id };
            configureBaseModel(model);
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

        private void configureBaseModel(BaseViewModel model)
        {
            model.IsAuthorized = HttpContext.User.Identity.IsAuthenticated;
            model.UserName = HttpContext.User.Identity.Name;
        }
    }
}