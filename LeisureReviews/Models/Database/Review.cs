using LeisureReviews.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        public User Author { get; set; }

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

        public string IllustrationId { get; set; }

        public ICollection<User> LikedUsers { get; set; } = new List<User>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [Required]
        [BindNever]
        public DateTime CreateTime { get; set; }

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();

        public bool IsDeleted { get; set; } = false;
    }
}
