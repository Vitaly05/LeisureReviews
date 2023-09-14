using LeisureReviews.Models.Database;

namespace LeisureReviews.Models.ViewModels
{
    public class ReviewCardViewModel
    {
        public bool CanEdit { get; set; }

        public Review Review { get; set; }
    }
}
