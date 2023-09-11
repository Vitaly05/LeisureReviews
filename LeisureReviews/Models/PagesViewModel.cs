namespace LeisureReviews.Models
{
    public class PagesViewModel : BaseViewModel
    {
        public int Page { get; set; } = 0;

        public int PageSize { get; set; } = 5;

        public int PagesCount { get; set; } = 1;
    }
}
