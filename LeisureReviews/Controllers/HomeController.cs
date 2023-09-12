using LeisureReviews.Data;
using LeisureReviews.Models;
using LeisureReviews.Models.Database;
using LeisureReviews.Models.Search;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [HttpGet("Home/{sortTarget}/{sortType}")]
        public async Task<IActionResult> Index(string sortTarget, string sortType, int page = 0, int pageSize = 5)
        {
            var model = new ReviewsListViewModel { ReviewSortModel = getReviewSortModel(sortTarget, sortType) };
            await configureReviewsListViewModel(model, r => true, page, pageSize);
            await configureBaseModel(model);
            return View(model);
        }

        [HttpGet("Profile/{userName}")]
        public async Task<IActionResult> Profile(string userName, int page = 0, int pageSize = 5)
        {
            var user = await usersRepository.FindAsync(userName);
            if (user is null) return NotFound();
            var model = new ProfileViewModel { User = user, LikesCount = await likesRepository.GetCountAsync(user) };
            await configureReviewsListViewModel(model, r => r.AuthorId == user.Id, page, pageSize);
            await configureBaseModel(model);
            if (model.CurrentUser is not null)
                model.CurrentUser.Roles = await usersRepository.GetRolesAsync(model.CurrentUser);
            return View(model);
        }

        [HttpGet("Profile/{userName}/{sortTarget}/{sortType}")]
        public async Task<IActionResult> Profile(string userName, string sortTarget, string sortType, int page = 0, int pageSize = 5)
        {
            var user = await usersRepository.FindAsync(userName);
            if (user is null) return NotFound();
            var model = new ProfileViewModel { User = user, LikesCount = await likesRepository.GetCountAsync(user), ReviewSortModel = getReviewSortModel(sortTarget, sortType) };
            await configureReviewsListViewModel(model, r => r.AuthorId == user.Id, page, pageSize);
            await configureBaseModel(model);
            if (model.CurrentUser is not null)
                model.CurrentUser.Roles = await usersRepository.GetRolesAsync(model.CurrentUser);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("AdminPanel")]
        public async Task<IActionResult> AdminPanel(int page = 0, int pageSize = 10)
        {
            var model = new AdminPanelViewModel()
            {
                Users = await usersRepository.GetAllAsync(page, pageSize)
            };
            foreach (var user in model.Users)
                user.LikesCount = await likesRepository.GetCountAsync(user);
            await configureBaseModel(model);
            configurePagesViewModel(model, page, pageSize, await usersRepository.GetPagesCountAsync(pageSize));
            return View(model);
        }

        private async Task configureReviewsListViewModel(ReviewsListViewModel model, Expression<Func<Review, bool>> predicate, int page, int pageSize)
        {
            configurePagesViewModel(model, page, pageSize, await reviewsRepository.GetPagesCountAsync(pageSize, predicate));
            model.Reviews = await getReviewsAsync(model.ReviewSortModel, predicate, page, pageSize);
            foreach (var review in model.Reviews)
                review.AverageRate = await ratesRepository.GetAverageRateAsync(review);
        }

        private void configurePagesViewModel(PagesViewModel model, int page, int pageSize, int pagesCount)
        {
            model.Page = page;
            model.PageSize = pageSize;
            model.PagesCount = pagesCount;
        }

        private async Task<List<Review>> getReviewsAsync(ReviewSortModel sortModel, Expression<Func<Review, bool>> predicate, int page, int pageSize) =>
            sortModel.Target switch
            {
                ReviewSortTarget.Date => await reviewsRepository.GetLatestAsync(predicate, sortModel.Type, page, pageSize),
                ReviewSortTarget.Rate => await reviewsRepository.GetTopRatedAsync(predicate, sortModel.Type, page, pageSize),
                ReviewSortTarget.Likes => await reviewsRepository.GetTopLikedAsync(predicate, sortModel.Type, page, pageSize),
                _ => new()
            };

        private ReviewSortModel getReviewSortModel(string sortTarget, string sortType)
        {
            Enum.TryParse(sortTarget, out ReviewSortTarget target);
            Enum.TryParse(sortType, out SortType type);
            return new ReviewSortModel { Target = target, Type = type };
        }
    }
}