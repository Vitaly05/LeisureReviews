using LeisureReviews.Models.Database;
using LeisureReviews.Repositories.interfaces;
using Microsoft.AspNetCore.Identity;

namespace LeisureReviews.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UserManager<User> userManager;

        public UsersRepository(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public Task<IdentityResult> CreateAsync(User user, string password) => 
            userManager.CreateAsync(user, password);
    }
}
