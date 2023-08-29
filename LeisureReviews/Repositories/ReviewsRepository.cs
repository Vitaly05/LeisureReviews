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
            return await context.Reviews.Where(r => r.AuthorId == authorId).ToListAsync();
        }

        public async Task<Review> GetAsync(string id)
        {
            return await context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        }

        public void Save(Review review)
        {
            if (context.Reviews.Any(r => r.Id == review.Id))
            {
                context.Reviews.Update(review);
                context.Entry(review).Property(r => r.CreateTime).IsModified = false;
                context.Entry(review).Property(r => r.AuthorId).IsModified = false;
            }
            else
            {
                initReview(review);
                context.Reviews.Add(review);
            }
            context.SaveChanges();
        }

        public async Task DeleteAsync(string id)
        {
            var review = await context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            review.IsDeleted = true;
            context.SaveChanges();
        }

        private void initReview(Review review)
        {
            review.CreateTime = DateTime.Now;
            review.Id = Guid.NewGuid().ToString();
        }
    }
}
