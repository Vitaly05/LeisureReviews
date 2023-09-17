using LeisureReviews.Models.Database;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface IIllustrationsRepository
    {
        Task AddAsync(string reviewId, IFormFile file);

        Task DeleteAllAsync(string reviewId);
    }
}
