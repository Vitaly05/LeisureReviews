using LeisureReviews.Models.Database;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface IReviewsRepository
    {
        void SaveReview(Review review);
    }
}
