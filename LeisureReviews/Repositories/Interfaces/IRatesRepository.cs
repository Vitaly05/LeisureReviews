using LeisureReviews.Models.Database;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface IRatesRepository
    {
        Task<Rate> GetAsync(User user, Leisure leisure);

        Task<double> GetAverageRateAsync(Leisure leisure);

        Task SaveAsync(Rate rate);
    }
}
