using LeisureReviews.Models.Database;

namespace LeisureReviews.Services.Interfaces
{
    public interface ISearchService
    {
        Task CreateReviewAsync(Review review);

        Task UpdateReviewAsync(Review review);

        Task DeleteReviewAsync(Review review);

        Task CreateLeisureAsync(Leisure leisure);
    }
}
