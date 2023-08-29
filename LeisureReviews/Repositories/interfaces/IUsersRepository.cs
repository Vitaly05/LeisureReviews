using LeisureReviews.Models.Database;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task<IdentityResult> CreateAsync(User user, string password);

        Task<IdentityResult> CreateAsync(User user);

        Task<User> FindAsync(string userName);

        Task<User> FindAsync(string externalProvider, string providerKey);

        Task<User> GetAsync(ClaimsPrincipal principal);
    }
}
