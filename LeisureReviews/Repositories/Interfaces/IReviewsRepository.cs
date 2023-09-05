using LeisureReviews.Models.Database;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface IReviewsRepository
    {
        Task<List<Review>> GetAllAsync(string authorId);

        Task<Review> GetAsync(string id);

        Task<List<Review>> GetLatestAsync(int page, int pageSize);

        Task<int> GetPagesCountAsync(int pageSize);

        Task SaveAsync(Review review);

        Task DeleteAsync(string id);
    }
}
