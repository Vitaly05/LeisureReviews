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

        private readonly ICloudService cloudService;

        public ReviewController(IUsersRepository usersRepository, IReviewsRepository reviewsRepository, 
            ITagsRepository tagsRepository, ICloudService cloudService)
        {
            this.usersRepository = usersRepository;
            this.reviewsRepository = reviewsRepository;
            this.tagsRepository = tagsRepository;
            this.cloudService = cloudService;
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
        public async Task<IActionResult> NewReview()
        {
            var model = new ReviewEditorViewModel();
            await configureReviewEditorViewModel(model);
            model.Review = new Review { AuthorId = model.CurrentUser.Id };
            return View("ReviewEditor", model);
        }

        [Authorize]
        [HttpGet("Edit")]
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

        private async Task updateIllustrationAsync(ReviewModel model)
        {
            if (model.IllustrationDeleted) model.IllustrationId = null;
            if (model.Illustration is not null)
                model.IllustrationId = await uploadIllustrationAsync(model.Illustration);
        }

        private async Task<string> uploadIllustrationAsync(IFormFile illustration)
        {
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
