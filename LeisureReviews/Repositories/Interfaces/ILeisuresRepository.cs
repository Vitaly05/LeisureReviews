using LeisureReviews.Models.Database;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface ILeisuresRepository
    {
        Task<Leisure> GetAsync(string id);

        Task<Leisure> GetFromReviewAsync(string reviewId);

        Task<Leisure> AddAsync(string name);
    }
}
