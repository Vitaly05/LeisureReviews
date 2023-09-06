using System.ComponentModel.DataAnnotations;

namespace LeisureReviews.Models.Database
{
    public class Rate
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Range(0, 5)]
        public int Value { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public Review Review { get; set; }
    }
}
