using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeisureReviews.Repositories
{
    public class LikesRepository : ILikesRepository
    {
        private readonly ApplicationContext context;

        public LikesRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task LikeAsync(Review review, User user)
        {
            if (context.Likes.Any(l => l.User.Id == user.Id && l.Review.Id == review.Id)) return;
            await context.Likes.AddAsync(new Like() { User = user, Review = review });
            await context.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync(User user)
        {
            var userReviews = (await context.Users.Include(u => u.AuthoredReviews).ThenInclude(r => r.Likes).FirstOrDefaultAsync(u => u.Id == user.Id)).AuthoredReviews;
            return userReviews.Sum(r => r.Likes.Count());
        }
    }
}
