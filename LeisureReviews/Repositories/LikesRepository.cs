using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeisureReviews.Repositories
{
    public class LikesRepository : BaseRepository, ILikesRepository
    {
        public LikesRepository(ApplicationContext context) : base(context) { }

        public async Task LikeAsync(Review review, User user)
        {
            if (context.Likes.Any(l => l.User.Id == user.Id && l.Review.Id == review.Id)) return;
            await context.Likes.AddAsync(new Like() { User = user, Review = review });
            await context.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync(User user) =>
            (await context.Users.Include(u => u.AuthoredReviews).ThenInclude(r => r.Likes).AsSplitQuery().FirstOrDefaultAsync(u => u.Id == user.Id))
                .AuthoredReviews.Sum(r => r.Likes.Count);
    }
}
