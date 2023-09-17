using System.ComponentModel.DataAnnotations;

namespace LeisureReviews.Models.Database
{
    public class Illustration
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string ReviewId { get; set; }

        public Review Review { get; set; }
    }
}
