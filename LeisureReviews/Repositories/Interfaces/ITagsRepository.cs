using LeisureReviews.Models.Database;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface ITagsRepository
    {
        Task<List<Tag>> GetTagsAsync();

        Task<ICollection<Tag>> GetTagsAsync(IEnumerable<string> tagsNames);

        void AddNewTags(IEnumerable<string> tagsNames);
    }
}
