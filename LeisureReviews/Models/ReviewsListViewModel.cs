using LeisureReviews.Models.Database;

namespace LeisureReviews.Models
{
    public class ReviewsListViewModel : BaseViewModel
    {
        public int Page { get; set; } = 0;

        public int PageSize { get; set; } = 5;

        public int PagesCount { get; set; } = 1;

        public List<Review> Reviews { get; set; }
    }
}
