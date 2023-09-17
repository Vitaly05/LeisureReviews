using LeisureReviews.Data;
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

        Task<User> GetWithoutQueryFiltersAsync(ClaimsPrincipal principal);

        Task<List<User>> GetAllAsync(int page, int pageSize);

        Task<int> GetPagesCountAsync(int pageSize);

        Task<List<string>> GetRolesAsync(User user);

        Task<string> GetUserNameAsync(string id);

        Task ChangeStatusAsync(User user, AccountStatus status);
    }
}
