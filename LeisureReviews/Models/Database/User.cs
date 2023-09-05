using Microsoft.AspNetCore.Identity;

namespace LeisureReviews.Models.Database
{
    public class User : IdentityUser
    {
        public string ExternalProvider { get; set; }

        public string ProviderKey { get; set; }

        public ICollection<Review> AuthoredReviews { get; set; } = new List<Review>();

        public ICollection<Review> LikedReviews { get; set; } = new List<Review>();
    }
}
