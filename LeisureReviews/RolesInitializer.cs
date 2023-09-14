using LeisureReviews.Data;
using LeisureReviews.Models.Database;
using Microsoft.AspNetCore.Identity;
namespace LeisureReviews
{
    public class RolesInitializer
    {
        private readonly UserManager<User> userManager;

        private readonly RoleManager<IdentityRole> roleManager;
        
        private readonly IConfiguration configuration;

        public RolesInitializer(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
        }

        public async Task InitializeAsync()
        {
            await initializeRolesAsync();
            await initializeAdminAsync();
        }

        private async Task initializeRolesAsync()
        {
            if (await roleManager.FindByNameAsync(configuration.GetSection("AdminData").GetValue<string>("UserName")) is null)
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
        }

        private async Task initializeAdminAsync()
        {
            var adminUserName = configuration.GetSection("AdminData").GetValue<string>("UserName");
            if (await userManager.FindByNameAsync(adminUserName) is null)
                await createAdminWithRoleAsync(adminUserName, configuration.GetSection("AdminData").GetValue<string>("Password"));
        }

        private async Task createAdminWithRoleAsync(string userName, string password)
        {
            var admin = new User { UserName = userName };
            IdentityResult result = await userManager.CreateAsync(admin, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, Roles.Admin.ToString());
        }
    }
}
