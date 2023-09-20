using AutoMapper;
using LeisureReviews.Models.Database;
using LeisureReviews.Services.Interfaces;
using Algolia.Search.Clients;
using LeisureReviews.Models.Search;

namespace LeisureReviews.Services
{
    public class AlgoliaSearchService : ISearchService
    {
        private readonly ISearchIndex reviewSearchIndex;

        private readonly ISearchIndex leisureSearchIndex;

        public AlgoliaSearchService(ISearchClient searchClient)
        {
            this.reviewSearchIndex = searchClient.InitIndex("reviews");
            this.leisureSearchIndex = searchClient.InitIndex("leisures");
        }

        public async Task CreateReviewAsync(Review review) =>
            await reviewSearchIndex.SaveObjectAsync(getSearchModel(review));


        public async Task UpdateReviewAsync(Review review) =>
            await reviewSearchIndex.PartialUpdateObjectAsync(getSearchModel(review));

        public async Task DeleteReviewAsync(Review review) =>
            await reviewSearchIndex.DeleteObjectAsync(review.Id);

        public async Task CreateLeisureAsync(Leisure leisure) =>
            await leisureSearchIndex.SaveObjectAsync(new LeisureSearchModel { Name = leisure.Name });

        private ReviewSearchModel getSearchModel(Review review) =>
            new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<Review, ReviewSearchModel>()
                .ForMember(r => r.Comments, opt => opt.MapFrom(r => getSearchModels<Comment, CommentSearchModel>(r.Comments)))
                .ForMember(r => r.Tags, opt => opt.MapFrom(r => getSearchModels<Tag, TagSearchModel>(r.Tags)))
                .ForMember(r => r.Leisure, opt => opt.MapFrom(r => r.Leisure.Name))
            )).Map<ReviewSearchModel>(review);

        private IEnumerable<TDestination> getSearchModels<TSourse, TDestination>(IEnumerable<TSourse> sourses)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<TSourse, TDestination>()));
            foreach (var sourse in sourses)
                yield return mapper.Map<TDestination>(sourse);
        }
    }
}
