using LeisureReviews.Models.Database;

namespace LeisureReviews.Models
{
    public class ReviewModel : Review
    {
        public List<string> TagsNames { get; set; }

        public IFormFile Illustration { get; set; }

        public bool IllustrationDeleted { get; set; }
    }
}
