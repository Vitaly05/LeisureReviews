using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;

namespace LeisureReviews.Repositories
{
    public class CommentsRepository : ICommentsRepository
    {
        private readonly ApplicationContext context;

        public CommentsRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task SaveAsync(Comment comment)
        {
            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();
        }
    }
}
