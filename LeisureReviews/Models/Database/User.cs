using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeisureReviews.Models.Database
{
    public class User : IdentityUser
    {
        public string ExternalProvider { get; set; }

        public string ProviderKey { get; set; }

        public ICollection<Review> AuthoredReviews { get; set; } = new List<Review>();

        public ICollection<Like> Likes { get; set; } = new List<Like>();

        public ICollection<Rate> Rates { get; set; }

        [NotMapped]
        public int LikesCount { get; set; }

        [NotMapped]
        public List<string> Roles { get; set; } = new List<string>();
    }
}
