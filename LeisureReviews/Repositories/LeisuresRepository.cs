using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeisureReviews.Repositories
{
    public class LeisuresRepository : BaseRepository, ILeisuresRepository
    {
        public LeisuresRepository(ApplicationContext context) : base(context) { }

        public async Task<Leisure> AddAsync(string name)
        {
            if (await context.Leisures.AnyAsync(l => l.Name == name)) 
                return await context.Leisures.FirstOrDefaultAsync(l => l.Name == name);
            var leisure = new Leisure { Name = name };
            await saveAsync(leisure);
            return leisure;
        }

        public async Task<Leisure> GetAsync(string id) =>
            await context.Leisures.FirstOrDefaultAsync(l => l.Id == id);

        public async Task<Leisure> GetFromReviewAsync(string reviewId) =>
            await context.Reviews.Where(r => r.Id == reviewId).Select(r => r.Leisure).FirstOrDefaultAsync();

        private async Task saveAsync(Leisure leisure)
        {
            await context.Leisures.AddAsync(leisure);
            await context.SaveChangesAsync();
        }
    }
}
