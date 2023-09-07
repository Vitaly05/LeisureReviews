using LeisureReviews.Models.Database;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface ILikesRepository
    {
        Task LikeAsync(Review review, User likedUser);

        Task<int> GetCountAsync(User user);
    }
}
