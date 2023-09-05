using LeisureReviews.Models.Database;

namespace LeisureReviews.Models
{
    public class ProfileViewModel : ReviewsListViewModel
    {
        public User User { get; set; }

        public bool CanEdit
        {
            get => User.UserName == CurrentUser?.UserName;
        }
    }
}
