using LeisureReviews.Models;
using LeisureReviews.Models.Database;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface ITagsRepository
    {
        Task<List<Tag>> GetTagsAsync();

        Task<ICollection<Tag>> GetTagsAsync(IEnumerable<string> tagsNames);

        Task<List<TagWeightModel>> GetWeightsAsync();

        void AddNewTags(IEnumerable<string> tagsNames);
    }
}
