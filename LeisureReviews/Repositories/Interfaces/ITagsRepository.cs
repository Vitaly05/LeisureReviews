using LeisureReviews.Models;
using LeisureReviews.Models.Database;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface ITagsRepository
    {
        Task<List<Tag>> GetAsync();

        Task<ICollection<Tag>> GetAsync(IEnumerable<string> tagsNames);

        Task<List<TagWeightModel>> GetWeightsAsync();

        Task AddNewAsync(IEnumerable<string> tagsNames);
    }
}
