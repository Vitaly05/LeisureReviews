using LeisureReviews.Models.Database;
using System.ComponentModel.DataAnnotations;

namespace LeisureReviews.Models
{
    public class ReviewEditorViewModel : BaseViewModel
    {
        public string AuthorName { get; set; }

        public Review Review { get; set; }
    }
}
