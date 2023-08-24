using System.ComponentModel.DataAnnotations;

namespace LeisureReviews.Models
{
    public class AdditionalInfoModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
