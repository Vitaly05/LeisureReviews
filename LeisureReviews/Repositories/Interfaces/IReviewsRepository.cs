using LeisureReviews.Models.Database;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface IReviewsRepository
    {
        Task<List<Review>> GetAllAsync(string authorId);

        Task<Review> GetAsync(string id);

        void Save(Review review);

        Task DeleteAsync(string id);
    }
}
