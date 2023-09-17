using LeisureReviews.Models.Database;

namespace LeisureReviews.Models
{
    public class ReviewModel : Review
    {
        public List<string> TagsNames { get; set; }

        public List<IFormFile> IllustrationsFiles { get; set; }

        public bool IllustrationChanged { get; set; }
    }
}
