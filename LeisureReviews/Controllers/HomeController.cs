using LeisureReviews.Models;
using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LeisureReviews.Controllers
{
    [Route("")]
    public class HomeController : BaseController
    {
        private readonly IReviewsRepository reviewsRepository;

        private readonly IRatesRepository ratesRepository;

        private readonly ILikesRepository likesRepository;

        public HomeController(IUsersRepository usersRepository, IReviewsRepository reviewsRepository,
            IRatesRepository ratesRepository, ILikesRepository likesRepository)
        {
            this.usersRepository = usersRepository;
            this.reviewsRepository = reviewsRepository;
            this.ratesRepository = ratesRepository;
            this.likesRepository = likesRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 0, int pageSize = 5)
        {
            var model = new ReviewsListViewModel();
            await configureReviewsListViewModel(model, (r) => true, page, pageSize);
            await configureBaseModel(model);
            return View(model);
        }

        [HttpGet("Profile/{userName}")]
        public async Task<IActionResult> Profile(string userName, int page = 0, int pageSize = 5)
        {
            var user = await usersRepository.FindAsync(userName);
            if (user is null) return NotFound();
            var model = new ProfileViewModel { User = user, LikesCount = await likesRepository.GetCountAsync(user) };
            await configureReviewsListViewModel(model, (r) => r.AuthorId == user.Id, page, pageSize);
            await configureBaseModel(model);
            return View(model);
        }

        private async Task configureReviewsListViewModel(ReviewsListViewModel model, Expression<Func<Review, bool>> predicate, int page, int pageSize)
        {
            model.Page = page;
            model.PageSize = pageSize;
            model.PagesCount = await reviewsRepository.GetPagesCountAsync(pageSize);
            model.Reviews = await reviewsRepository.GetLatestAsync(predicate, page, pageSize);
            foreach (var review in model.Reviews)
                review.AverageRate = await ratesRepository.GetAverageRateAsync(review);
        }
    }
}