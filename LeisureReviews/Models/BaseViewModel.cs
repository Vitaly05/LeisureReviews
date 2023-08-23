namespace LeisureReviews.Models
{
    public class BaseViewModel
    {
        public bool IsAuthorized { get; set; } = false;

        public string UserName { get; set; } = "";
    }
}
