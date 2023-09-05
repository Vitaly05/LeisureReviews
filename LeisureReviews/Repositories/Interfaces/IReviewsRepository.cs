using LeisureReviews.Models.Database;
using System.Linq.Expressions;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface IReviewsRepository
    {
        Task<List<Review>> GetAllAsync(string authorId);

        Task<Review> GetAsync(string id);

        Task<List<Review>> GetLatestAsync(Expression<Func<Review, bool>> predicate, int page, int pageSize);

        Task<int> GetPagesCountAsync(int pageSize);

        Task SaveAsync(Review review);

        Task DeleteAsync(string id);

        Task LikeAsync(string reviewId, User likedUser);
    }
}
