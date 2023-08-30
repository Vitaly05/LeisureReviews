using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LeisureReviews.Models.Database
{
    [Index(nameof(Name), IsUnique = true)]
    public class Tag
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
