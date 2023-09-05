using AutoMapper;
using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeisureReviews.Repositories
{
    public class ReviewsRepository : IReviewsRepository
    {
        private readonly ApplicationContext context;

        public ReviewsRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<List<Review>> GetAllAsync(string authorId)
        {
            var f = context.Reviews.Where(r => r.AuthorId == authorId).Select(r => r.Tags);
            var d = await context.Reviews.Where(r => r.AuthorId == authorId).ToListAsync();
            return await context.Reviews.Where(r => r.AuthorId == authorId).Include(r => r.Tags).ToListAsync();
        }

        public async Task<Review> GetAsync(string id) =>
            await context.Reviews.Include(r => r.Tags).FirstOrDefaultAsync(r => r.Id == id);

        public async Task<List<Review>> GetLatestAsync(int page, int pageSize) =>
            await context.Reviews.Include(r => r.Tags).OrderByDescending(r => r.CreateTime).Skip(page * pageSize).Take(pageSize).ToListAsync();

        public async Task<int> GetPagesCountAsync(int pageSize) =>
            (int)Math.Ceiling(await context.Reviews.CountAsync() / (double)pageSize);

        public async Task SaveAsync(Review review)
        {
            if (context.Reviews.Any(r => r.Id == review.Id))
                await updateReview(review);
            else
                addReview(review);
            context.SaveChanges();
        }

        public async Task DeleteAsync(string id)
        {
            var review = await context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            review.IsDeleted = true;
            context.SaveChanges();
        }

        private async Task updateReview(Review review)
        {
            var updatedReview = getUpdatedReview(await GetAsync(review.Id), review);
            context.Entry(updatedReview).Property(r => r.CreateTime).IsModified = false;
            context.Entry(updatedReview).Property(r => r.AuthorId).IsModified = false;
            context.Reviews.Update(updatedReview);
        }

        private Review getUpdatedReview(Review existingReview, Review updatedReview)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Review, Review>());
            var mapper = new Mapper(config);
            return mapper.Map(updatedReview, existingReview);
        }

        private void addReview(Review review)
        {
            review.CreateTime = DateTime.Now;
            review.Id = Guid.NewGuid().ToString();
            context.Reviews.Add(review);
        }
    }
}
