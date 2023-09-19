using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeisureReviews.Models.Database
{
    [Index(nameof(Name), IsUnique = true)]
    public class Leisure
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<Rate> Rates { get; set; } = new List<Rate>();

        [NotMapped]
        public double AverageRate { get; set; }
    }
}
