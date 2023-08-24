using Microsoft.AspNetCore.Identity;

namespace LeisureReviews.Models.Database
{
    public class User : IdentityUser
    {
        public string ExternalProvider { get; set; }

        public string ProviderKey { get; set; }
    }
}
