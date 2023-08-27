using LeisureReviews.Models.Database;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LeisureReviews.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task<IdentityResult> CreateAsync(User user, string password);

        Task<IdentityResult> CreateAsync(User user);

        Task<User> FindUserAsync(string userName);

        Task<User> FindUserAsync(string externalProvider, string providerKey);

        Task<User> GetUserAsync(ClaimsPrincipal principal);
    }
}
