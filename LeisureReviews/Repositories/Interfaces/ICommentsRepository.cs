using LeisureReviews.Models.Database;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface ICommentsRepository
    {
        Task SaveAsync(Comment comment);
    }
}
