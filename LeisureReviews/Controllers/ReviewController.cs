using LeisureReviews.Models.Database;
using LeisureReviews.Models;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeisureReviews.Services.Interfaces;

namespace LeisureReviews.Controllers
{
    [Route("[Controller]")]
    public class ReviewController : BaseController
    {
        private readonly IReviewsRepository reviewsRepository;

        private readonly ITagsRepository tagsRepository;

        private readonly IRatesRepository ratesRepository;
        
        private readonly ILikesRepository likesRepository;

        private readonly ICloudService cloudService;

        public ReviewController(IUsersRepository usersRepository, IReviewsRepository reviewsRepository, 
            ITagsRepository tagsRepository, IRatesRepository ratesRepository, ILikesRepository likesRepository,
            ICloudService cloudService)
        {
            this.usersRepository = usersRepository;
            this.reviewsRepository = reviewsRepository;
            this.tagsRepository = tagsRepository;
            this.ratesRepository = ratesRepository;
            this.likesRepository = likesRepository;
            this.cloudService = cloudService;
        }

        public async Task<IActionResult> Index(string reviewId)
        {
            if (reviewId is null) return BadRequest();
            var model = new ReviewViewModel();
            await configureBaseModel(model);
            model.Review = await reviewsRepository.GetAsync(reviewId);
            if (model.Review is null) return NotFound();
            model.Review.Author.LikesCount = await likesRepository.GetCountAsync(model.Review.Author);
            foreach (var comment in model.Review.Comments)
                comment.Author.LikesCount = await likesRepository.GetCountAsync(comment.Author);
            model.AverageRate = await ratesRepository.GetAverageRateAsync(model.Review);
            if (model.IsAuthorized) model.CurrentUserRate = await ratesRepository.GetAsync(model.CurrentUser, model.Review);
            return View(model);
        }

        [HttpGet("GetIllustration")]
        public async Task<IActionResult> GetIllustraton(string fileId)
        {
            if (fileId is null) return NotFound();
            var fileContent = await cloudService.GetAsync(fileId);
            return Ok($"data:image;base64,{Convert.ToBase64String(fileContent)}");
        }

        [Authorize]
        [HttpGet("New")]
        public async Task<IActionResult> NewReview(string authorId)
        {
            var model = new ReviewEditorViewModel();
            model.Review = new Review { AuthorId = authorId };
            await configureReviewEditorViewModel(model);
            return View("ReviewEditor", model);
        }

        [Authorize]
        [HttpGet("Edit")]
        public async Task<IActionResult> EditReview(string reviewId)
        {
            var model = new ReviewEditorViewModel();
            model.Review = await reviewsRepository.GetAsync(reviewId);
            if (model.Review is null) return NotFound();
            await configureReviewEditorViewModel(model);
            if (!await canEditAsync(model.CurrentUser, model.Review)) return Forbid();
            return View("ReviewEditor", model);
        }

        [Authorize]
        [HttpPost("Save")]
        public async Task<IActionResult> SaveReview(ReviewModel reviewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            if (reviewModel.TagsNames is not null) await addTagsAsync(reviewModel);
            await updateIllustrationAsync(reviewModel);
            await reviewsRepository.SaveAsync(reviewModel);
            reviewModel.Tags.Clear();
            return Ok(reviewModel);
        }

        [Authorize]
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteReview(string reviewId)
        {
            var review = await reviewsRepository.GetAsync(reviewId);
            if (review is null) return NotFound();
            if (!await canEditAsync(await getCurrentUser(), review)) return Forbid();
            await reviewsRepository.DeleteAsync(reviewId);
            return Ok(reviewId);
        }

        [Authorize]
        [HttpPost("{reviewId}/Like")]
        public async Task<IActionResult> LikeReview(string reviewId)
        {
            if (reviewId is null) return BadRequest();
            await likesRepository.LikeAsync(await reviewsRepository.GetAsync(reviewId), await getCurrentUser());
            return Ok();
        }

        [Authorize]
        [HttpPost("{reviewId}/Rate/{value}")]
        public async Task<IActionResult> RateReview(string reviewId, int value)
        {
            var rate = new Rate
            {
                Review = await reviewsRepository.GetAsync(reviewId),
                User = await getCurrentUser(),
                Value = value
            };
            await ratesRepository.SaveAsync(rate);
            return Ok(new { value = rate.Value, average = await ratesRepository.GetAverageRateAsync(rate.Review) });
        }

        private async Task<bool> canEditAsync(User user, Review review)
        {
            var isAdmin = (await usersRepository.GetRolesAsync(user)).Contains("Admin");
            return isAdmin ? true : review.AuthorId == user.Id;
        }

        private async Task addTagsAsync(ReviewModel model)
        {
            await tagsRepository.AddNewAsync(model.TagsNames);
            model.Tags = await tagsRepository.GetAsync(model.TagsNames);
        }

        private async Task configureReviewEditorViewModel(ReviewEditorViewModel model)
        {
            await configureBaseModel(model);
            model.AuthorName = await usersRepository.GetUserNameAsync(model.Review.AuthorId);
            model.Tags = await tagsRepository.GetAsync();
        }

        private async Task updateIllustrationAsync(ReviewModel model)
        {
            if (!model.IllustrationChanged) return;
            var oldIllustrationId = model.IllustrationId;
            model.IllustrationId = await uploadIllustrationAsync(model.Illustration);
            await cloudService.DeleteAsync(oldIllustrationId);
        }

        private async Task<string> uploadIllustrationAsync(IFormFile illustration)
        {
            if (illustration is null) return null;
            using (var reader = new StreamReader(illustration.OpenReadStream()))
            {
                var bytes = default(byte[]);
                using (var memoryStream = new MemoryStream())
                {
                    reader.BaseStream.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }
                return await cloudService.UploadAsync(bytes, Path.GetExtension(illustration.FileName));
            }
        }
    }
}
