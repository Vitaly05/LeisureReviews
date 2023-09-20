using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using LeisureReviews.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeisureReviews.Repositories
{
    public class CommentsRepository : BaseRepository, ICommentsRepository
    {
        private readonly ISearchService searchService;

        public CommentsRepository(ApplicationContext context, ISearchService searchService) : base(context)
        {
            this.searchService = searchService;
        }

        public async Task SaveAsync(Comment comment)
        {
            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();
            await searchService.UpdateReviewAsync(await context.Reviews.FirstOrDefaultAsync(r => r.Id == comment.Review.Id));
        }
    }
}
