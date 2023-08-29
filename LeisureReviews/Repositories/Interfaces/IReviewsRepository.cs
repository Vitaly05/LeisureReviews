using LeisureReviews.Models.Database;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface IReviewsRepository
    {
        Task<List<Review>> GetAll(string authorId);

        Task<Review> Get(string id);

        void SaveReview(Review review);
    }
}
