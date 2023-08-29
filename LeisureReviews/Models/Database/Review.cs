using LeisureReviews.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeisureReviews.Models.Database
{
    public class Review
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string AuthorId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        [MaxLength(255)]
        public string Leisure { get; set; }

        [Required]
        public LeisureGroup Group { get; set; }

        [Required]
        [Range(0, 10)]
        public byte AuthorRate { get; set; }

        [Required]
        [Column(TypeName = "ntext")]
        public string Content { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
