using LeisureReviews.Models.Database;

namespace LeisureReviews.Models.ViewModels
{
    public class ReviewViewModel : BaseViewModel
    {
        public Review Review { get; set; }

        public List<Review> RelatedReviews { get; set; }

        public Rate CurrentUserRate { get; set; }

        public double AverageRate { get; set; }
    }
}
