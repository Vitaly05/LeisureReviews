﻿using LeisureReviews.Models.Database;
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

        public async Task<IdentityResult> CreateAsync(User user, string password) => 
            await userManager.CreateAsync(user, password);

        public async Task<IdentityResult> CreateAsync(User user) =>
            await userManager.CreateAsync(user);

        public async Task<User> FindUserAsync(string email) => 
            await userManager.FindByEmailAsync(email);
    }
}