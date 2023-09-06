using LeisureReviews.Models.Database;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface IRatesRepository
    {
        Task<Rate> GetAsync(User user, Review review);

        Task<double> GetAverageRateAsync(Review review);

        Task SaveAsync(Rate rate);
    }
}
