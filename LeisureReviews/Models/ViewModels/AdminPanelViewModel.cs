using LeisureReviews.Models.Database;

namespace LeisureReviews.Models.ViewModels
{
    public class AdminPanelViewModel : PagesViewModel
    {
        public List<User> Users { get; set; }
    }
}
