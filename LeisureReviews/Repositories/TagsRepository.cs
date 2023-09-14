using LeisureReviews.Models;
using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeisureReviews.Repositories
{
    public class TagsRepository : BaseRepository, ITagsRepository
    {
        public TagsRepository(ApplicationContext context) : base(context) { }

        public async Task<List<Tag>> GetAsync() =>
            await context.Tags.ToListAsync();

        public async Task<ICollection<Tag>> GetAsync(IEnumerable<string> tagsNames) =>
            await context.Tags.Where(t => tagsNames.Contains(t.Name)).ToListAsync();

        public async Task<List<TagWeightModel>> GetWeightsAsync() =>
            await context.Tags.Where(r => r.Reviews.Count() > 0).Select(t => new TagWeightModel { Text = t.Name, Weight = t.Reviews.Count(), Link = $"/Home/{t.Name}" }).ToListAsync();

        public async Task AddNewAsync(IEnumerable<string> tagsNames)
        {
            foreach (var tagName in tagsNames)
                if (!await context.Tags.AnyAsync(t => t.Name == tagName))
                    await context.Tags.AddAsync(new Tag() { Id = Guid.NewGuid().ToString(), Name = tagName });
            await context.SaveChangesAsync();
        }
    }
}
