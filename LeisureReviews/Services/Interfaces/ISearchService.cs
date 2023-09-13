using LeisureReviews.Models.Database;

namespace LeisureReviews.Services.Interfaces
{
    public interface ISearchService
    {
        Task CreateAsync(Review review);

        Task UpdateAsync(Review review);

        Task DeleteAsync(Review review);
    }
}
