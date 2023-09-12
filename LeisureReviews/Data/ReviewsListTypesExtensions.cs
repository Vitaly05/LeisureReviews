using LeisureReviews.Models.Database;
using System.Linq.Expressions;

namespace LeisureReviews.Data
{
    public static class ReviewsListTypesExtensions
    {
        public static Dictionary<ReviewsListType, string> StringValues = new()
        {
            { ReviewsListType.Latest, "Latest reviews" },
            { ReviewsListType.TopRated, "Top-rated reviews" },
        };
    }
}
