﻿using LeisureReviews.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeisureReviews.Models.Database
{
    public class Review
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

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

        public ICollection<Like> Likes { get; set; } = new List<Like>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Rate> Rates { get; set; } = new List<Rate>();

        [NotMapped]
        public double AverageRate { get; set; }

        [Required]
        [BindNever]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();

        public bool IsDeleted { get; set; } = false;
    }
}
