using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LeisureReviews.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UserManager<User> userManager;

        public UsersRepository(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IdentityResult> CreateAsync(User user, string password) => 
            await userManager.CreateAsync(user, password);

        public async Task<IdentityResult> CreateAsync(User user) =>
            await userManager.CreateAsync(user);

        public async Task<User> FindAsync(string userName) =>
            await userManager.FindByNameAsync(userName);

        public async Task<User> FindAsync(string externalProvider, string providerKey) => 
            await userManager.Users.FirstOrDefaultAsync(u => u.ExternalProvider == externalProvider && u.ProviderKey == providerKey);

        public async Task<User> GetAsync(ClaimsPrincipal principal) =>
            await userManager.GetUserAsync(principal);
    }
}
