﻿using LeisureReviews.Models.Database;
using LeisureReviews.Models;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeisureReviews.Services.Interfaces;
using LeisureReviews.Models.ViewModels;

namespace LeisureReviews.Controllers
{
    [Route("[Controller]")]
    public class ReviewController : BaseController
    {
        private readonly IReviewsRepository reviewsRepository;

        private readonly ITagsRepository tagsRepository;

        private readonly IRatesRepository ratesRepository;
        
        private readonly ILikesRepository likesRepository;

        private readonly IIllustrationsRepository illustrationsRepository;

        private readonly ILeisuresRepository leisuresRepository;

        private readonly ICloudService cloudService;

        public ReviewController(IUsersRepository usersRepository, IReviewsRepository reviewsRepository, 
            ITagsRepository tagsRepository, IRatesRepository ratesRepository, ILikesRepository likesRepository,
            IIllustrationsRepository illustrationsRepository, ILeisuresRepository leisuresRepository,
            ICloudService cloudService) : base(usersRepository)
        {
            this.reviewsRepository = reviewsRepository;
            this.tagsRepository = tagsRepository;
            this.ratesRepository = ratesRepository;
            this.likesRepository = likesRepository;
            this.illustrationsRepository = illustrationsRepository;
            this.leisuresRepository = leisuresRepository;
            this.cloudService = cloudService;
        }

        public async Task<IActionResult> Index(string reviewId)
        {
            if (reviewId is null) return BadRequest();
            var model = await initReviewViewModelAsync(reviewId);
            if (model is null) return NotFound();
            return View(model);
        }

        [HttpGet("GetIllustration")]
        public async Task<IActionResult> GetIllustraton(string fileId)
        {
            if (fileId is null) return NotFound();
            var fileContent = await cloudService.GetAsync(fileId);
            return Ok($"data:image/jpeg;base64,{Convert.ToBase64String(fileContent)}");
        }

        [Authorize]
        [HttpGet("New")]
        public async Task<IActionResult> NewReview(string authorId)
        {
            var model = new ReviewEditorViewModel();
            model.Review = new Review { AuthorId = authorId };
            await configureReviewEditorViewModelAsync(model);
            return View("ReviewEditor", model);
        }

        [Authorize]
        [HttpGet("Edit")]
        public async Task<IActionResult> EditReview(string reviewId)
        {
            var model = new ReviewEditorViewModel { Review = await reviewsRepository.GetAsync(reviewId) };
            if (model.Review is null) return NotFound();
            await configureReviewEditorViewModelAsync(model);
            if (!await canEditAsync(model.CurrentUser, model.Review)) return Forbid();
            return View("ReviewEditor", model);
        }

        [Authorize]
        [HttpPost("Save")]
        public async Task<IActionResult> SaveReview(ReviewModel reviewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            await saveReviewAsync(reviewModel);
            clearCycles(reviewModel);
            return Ok(reviewModel);
        }

        [Authorize]
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteReview(string reviewId)
        {
            var review = await reviewsRepository.GetAsync(reviewId);
            if (review is null) return NotFound();
            if (!await canEditAsync(await getCurrentUserAsync(), review)) return Forbid();
            await reviewsRepository.DeleteAsync(reviewId);
            return Ok(reviewId);
        }

        [Authorize]
        [HttpPost("{reviewId}/Like")]
        public async Task<IActionResult> LikeReview(string reviewId)
        {
            if (reviewId is null) return BadRequest();
            await likesRepository.LikeAsync(await reviewsRepository.GetAsync(reviewId), await getCurrentUserAsync());
            return Ok();
        }

        [Authorize]
        [HttpPost("{reviewId}/Rate/{value}")]
        public async Task<IActionResult> RateReview(string reviewId, int value)
        {
            var rate = new Rate
            {
                Leisure = await leisuresRepository.GetFromReviewAsync(reviewId),
                User = await getCurrentUserAsync(),
                Value = value
            };
            await ratesRepository.SaveAsync(rate);
            return Ok(new { value = rate.Value, average = await ratesRepository.GetAverageRateAsync(rate.Leisure) });
        }

        private async Task<bool> canEditAsync(User user, Review review)
        {
            var isAdmin = (await usersRepository.GetRolesAsync(user)).Contains("Admin");
            return isAdmin || review.AuthorId == user.Id;
        }

        private async Task saveReviewAsync(ReviewModel reviewModel)
        {
            if (reviewModel.TagsNames is not null) await addTagsAsync(reviewModel);
            reviewModel.Leisure = await leisuresRepository.AddAsync(reviewModel.LeisureName);
            reviewModel.LeisureId = reviewModel.Leisure.Id;
            await reviewsRepository.SaveAsync(reviewModel);
            await updateIllustrationAsync(reviewModel);
        }

        private async Task addTagsAsync(ReviewModel model)
        {
            await tagsRepository.AddNewAsync(model.TagsNames);
            model.Tags = await tagsRepository.GetAsync(model.TagsNames);
        }

        private async Task<ReviewViewModel> initReviewViewModelAsync(string reviewId)
        {
            var model = new ReviewViewModel { Review = await reviewsRepository.GetAsync(reviewId) };
            if (model.Review is null) return null;
            await configureReviewViewModelAsync(model);
            await configureRelatedReviewsAsync(model);
            return model;
        }

        private async Task configureReviewViewModelAsync(ReviewViewModel model)
        {
            model.Review.Author.LikesCount = await likesRepository.GetCountAsync(model.Review.Author);
            await configureCommentsAuthorsLikesCountAsync(model);
            model.AverageRate = await ratesRepository.GetAverageRateAsync(model.Review.Leisure);
            await configureBaseModelAsync(model);
            if (model.IsAuthorized) model.CurrentUserRate = await ratesRepository.GetAsync(model.CurrentUser, model.Review.Leisure);
        }

        private async Task configureRelatedReviewsAsync(ReviewViewModel model)
        {
            model.RelatedReviews = await reviewsRepository.GetRelatedAsync(model.Review.Id, 5);
            foreach (var review in model.RelatedReviews)
                review.Leisure.AverageRate = model.AverageRate;
        }

        private async Task configureCommentsAuthorsLikesCountAsync(ReviewViewModel model)
        {
            foreach (var comment in model.Review.Comments)
                comment.Author.LikesCount = await likesRepository.GetCountAsync(comment.Author);
        }

        private async Task configureReviewEditorViewModelAsync(ReviewEditorViewModel model)
        {
            await configureBaseModelAsync(model);
            model.AuthorName = await usersRepository.GetUserNameAsync(model.Review.AuthorId);
            model.Tags = await tagsRepository.GetAsync();
        }

        private async Task updateIllustrationAsync(ReviewModel model)
        {
            if (!model.IllustrationChanged) return;
            await illustrationsRepository.DeleteAllAsync(model.Id);
            if (model.IllustrationsFiles is not null && model.IllustrationsFiles.Any())
                await uploadIllustrationsAsync(model.IllustrationsFiles, model.Id);
        }

        private async Task uploadIllustrationsAsync(List<IFormFile> illustrationsFiles, string reviewId)
        {
            foreach (var file in illustrationsFiles)
                await illustrationsRepository.AddAsync(reviewId, file);
        }

        private void clearCycles(ReviewModel model)
        {
            model.Tags.Clear();
            model.Leisure.Reviews.Clear();
            model.Leisure.Rates.Clear();
            model.Illustrations.Clear();
            model.Author?.AuthoredReviews.Clear();
        }
    }
}
