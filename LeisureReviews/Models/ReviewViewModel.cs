using LeisureReviews.Models.Database;

namespace LeisureReviews.Models
{
    public class ReviewViewModel : BaseViewModel
    {
        public Review Review { get; set; }

        public Rate CurrentUserRate { get; set; }

        public double AverateRate { get; set; }
    }
}
