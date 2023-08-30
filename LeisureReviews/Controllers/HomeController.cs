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

        private readonly ITagsRepository tagsRepository;

        public HomeController(IUsersRepository usersRepository, IReviewsRepository reviewsRepository, ITagsRepository tagsRepository)
        {
            this.usersRepository = usersRepository;
            this.reviewsRepository = reviewsRepository;
            this.tagsRepository = tagsRepository;
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

        [Authorize]
        [HttpGet("NewReview")]
        public async Task<IActionResult> NewReview()
        {
            var model = new ReviewEditorViewModel();
            await configureReviewEditorViewModel(model);
            model.Review = new Review { AuthorId = model.CurrentUser.Id };
            return View("ReviewEditor", model);
        }

        [Authorize]
        [HttpGet("EditReview/{reviewId}")]
        public async Task<IActionResult> EditReview(string reviewId)
        {
            var model = new ReviewEditorViewModel();
            await configureReviewEditorViewModel(model);
            model.Review = await reviewsRepository.GetAsync(reviewId);
            if (model.Review is null) return NotFound();
            if (model.CurrentUser.Id != model.Review.AuthorId) return Forbid();
            return View("ReviewEditor", model);
        }

        [Authorize]
        [HttpPost("SaveReview")]
        public async Task<IActionResult> SaveReview(ReviewModel reviewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            if (reviewModel.TagsNames is not null) await addTagsAsync(reviewModel);
            await reviewsRepository.SaveAsync(reviewModel);
            reviewModel.Tags.Clear();
            return Ok(reviewModel);
        }

        [Authorize]
        [HttpDelete("DeleteReview")]
        public async Task<IActionResult> DeleteReview(string reviewId)
        {
            var review = await reviewsRepository.GetAsync(reviewId);
            if (review is null) return NotFound();
            if (review.AuthorId != (await getCurrentUser()).Id) return Forbid();
            await reviewsRepository.DeleteAsync(reviewId);
            return Ok(reviewId);
        }

        private async Task addTagsAsync(ReviewModel model)
        {
            tagsRepository.AddNewTags(model.TagsNames);
            model.Tags = await tagsRepository.GetTagsAsync(model.TagsNames);
        }

        private async Task configureReviewEditorViewModel(ReviewEditorViewModel model)
        {
            await configureBaseModel(model);
            model.AuthorName = model.CurrentUser.UserName;
            model.Tags = await tagsRepository.GetTagsAsync();
        }

        private async Task configureBaseModel(BaseViewModel model)
        {
            model.IsAuthorized = HttpContext.User.Identity.IsAuthenticated;
            model.CurrentUser = await getCurrentUser();
        }

        private async Task<User> getCurrentUser() =>
                    await usersRepository.GetAsync(HttpContext.User);
    }
}