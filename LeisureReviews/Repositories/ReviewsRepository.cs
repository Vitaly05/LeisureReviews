using AutoMapper;
using LeisureReviews.Data;
using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using LeisureReviews.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LeisureReviews.Repositories
{
    public class ReviewsRepository : BaseRepository, IReviewsRepository
    {
        private readonly ISearchService searchService;

        public ReviewsRepository(ApplicationContext context, ISearchService searchService) : base(context)
        {
            this.searchService = searchService;
        }

        public async Task<List<Review>> GetAllAsync(string authorId) =>
            await context.Reviews.Where(r => r.AuthorId == authorId).OrderByDescending(r => r.CreateTime).Include(r => r.Tags).Include(r => r.Leisure).AsSplitQuery().ToListAsync();

        public async Task<Review> GetAsync(string id) =>
            await context.Reviews.Include(r => r.Tags).Include(r => r.Author).Include(r => r.Likes).ThenInclude(l => l.User)
                .Include(r => r.Comments).ThenInclude(c => c.Author).Include(r => r.Illustrations).Include(r => r.Leisure).ThenInclude(l => l.Rates)
                .AsSplitQuery().FirstOrDefaultAsync(r => r.Id == id);

        public async Task<List<Review>> GetRelatedAsync(string reviewId, int count)
        {
            var leisureId = await context.Reviews.Where(r => r.Id == reviewId).Select(r => r.LeisureId).FirstOrDefaultAsync();
            return await context.Reviews.Where(r => r.LeisureId == leisureId).Where(r => r.Id != reviewId).OrderBy(x => Guid.NewGuid())
                .Take(count).Include(r => r.Leisure).AsSplitQuery().ToListAsync();
        }

        public async Task<List<Review>> GetLatestAsync(Expression<Func<Review, bool>> predicate, SortType sortType, int page, int pageSize)
        {
            IQueryable<Review> query = orderReviews(sortType, r => r.CreateTime);
            return await getPageAsync(query, predicate, page, pageSize);
        }

        public async Task<List<Review>> GetTopRatedAsync(Expression<Func<Review, bool>> predicate, SortType sortType, int page, int pageSize)
        {
            IQueryable<Review> query = orderReviews(sortType, r => r.Leisure.Rates.Average(r => r.Value));
            return await getPageAsync(query, predicate, page, pageSize);
        }

        public async Task<List<Review>> GetTopLikedAsync(Expression<Func<Review, bool>> predicate, SortType sortType, int page, int pageSize)
        {
            IQueryable<Review> query = orderReviews(sortType, r => r.Likes.Count);
            return await getPageAsync(query, predicate, page, pageSize);
        }

        public async Task<int> GetPagesCountAsync(int pageSize, Expression<Func<Review, bool>> predicate) =>
            (int)Math.Ceiling(await context.Reviews.Where(predicate).CountAsync() / (double)pageSize);

        public async Task SaveAsync(Review review)
        {
            if (context.Reviews.Any(r => r.Id == review.Id))
                await updateReviewAsync(review);
            else await addReviewAsync(review);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var review = await context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            review.IsDeleted = true;
            await searchService.DeleteAsync(review);
            context.SaveChanges();
        }

        private async Task<List<Review>> getPageAsync(IQueryable<Review> reviewQuery, Expression<Func<Review, bool>> predicate, int page, int pageSize) =>
            await reviewQuery.Where(predicate).Skip(page * pageSize).Take(pageSize).ToListAsync();

        private async Task updateReviewAsync(Review review)
        {
            var updatedReview = getUpdatedReview(await GetAsync(review.Id), review);
            context.Entry(updatedReview).Property(r => r.CreateTime).IsModified = false;
            context.Entry(updatedReview).Property(r => r.AuthorId).IsModified = false;
            context.Reviews.Update(updatedReview);
            await searchService.UpdateAsync(await GetAsync(updatedReview.Id));
        }

        private Review getUpdatedReview(Review existingReview, Review updatedReview)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Review, Review>()
                .ForMember(r => r.Comments, opt => opt.Ignore())
                .ForMember(r => r.Likes, opt => opt.Ignore())
                .ForMember(r => r.Illustrations, opt => opt.Ignore()));
            var mapper = new Mapper(config);
            return mapper.Map(updatedReview, existingReview);
        }

        private async Task addReviewAsync(Review review)
        {
            context.Reviews.Add(review);
            await searchService.CreateAsync(review);
        }

        private IQueryable<Review> orderReviews<TKey>(SortType sortType, Expression<Func<Review, TKey>> keySelector) =>
            sortType switch
            {
                SortType.Descending => context.Reviews.OrderByDescending(keySelector).Include(r => r.Tags).Include(r => r.Likes).Include(r => r.Leisure).AsSplitQuery(),
                _ => context.Reviews.OrderBy(keySelector).Include(r => r.Tags).Include(r => r.Likes).Include(r => r.Leisure).AsSplitQuery()
            };
    }
}
