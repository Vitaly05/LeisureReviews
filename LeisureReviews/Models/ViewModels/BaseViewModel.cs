using LeisureReviews.Models.Database;

namespace LeisureReviews.Models.ViewModels
{
    public class BaseViewModel
    {
        public bool IsAuthorized { get; set; } = false;

        public User CurrentUser { get; set; }

        public string AdditionalUrl { get; set; } = "";
    }
}
