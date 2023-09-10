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

        public AlgoliaSearchService(ISearchClient searchClient)
        {
            this.reviewSearchIndex = searchClient.InitIndex("reviews");
        }

        public async Task CreateAsync(Review review) =>
            await reviewSearchIndex.SaveObjectAsync(getSearchModel(review));

        public async Task UpdateAsync(Review review) =>
            await reviewSearchIndex.PartialUpdateObjectAsync(getSearchModel(review));

        private ReviewSearchModel getSearchModel(Review review) =>
            new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<Review, ReviewSearchModel>()
                .ForMember(r => r.Comments, opt => opt.MapFrom(r => getSearchModels<Comment, CommentSearchModel>(r.Comments)))
                .ForMember(r => r.Tags, opt => opt.MapFrom(r => getSearchModels<Tag, TagSearchModel>(r.Tags)))
            )).Map<ReviewSearchModel>(review);

        private IEnumerable<TDestination> getSearchModels<TSourse, TDestination>(IEnumerable<TSourse> sourses)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<TSourse, TDestination>()));
            foreach (var sourse in sourses)
                yield return mapper.Map<TDestination>(sourse);
        }
    }
}
