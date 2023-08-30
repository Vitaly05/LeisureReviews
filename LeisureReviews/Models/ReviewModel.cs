using LeisureReviews.Models.Database;

namespace LeisureReviews.Models
{
    public class ReviewModel : Review
    {
        public List<string> TagsNames { get; set; }
    }
}
