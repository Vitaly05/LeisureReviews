using LeisureReviews.Models.Database;

namespace LeisureReviews.Models
{
    public class BaseViewModel
    {
        public bool IsAuthorized { get; set; } = false;

        public User CurrentUser { get; set; }
    }
}
