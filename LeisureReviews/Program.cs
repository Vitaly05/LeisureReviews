using Algolia.Search.Clients;
using CloudinaryDotNet;
using LeisureReviews;
using LeisureReviews.Hubs;
using LeisureReviews.Middlewares;
using LeisureReviews.Models.Database;
using LeisureReviews.Repositories;
using LeisureReviews.Repositories.Interfaces;
using LeisureReviews.Services;
using LeisureReviews.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<ApplicationContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/SignIn";
    options.AccessDeniedPath = "/AccessDenied";
});

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration.GetSection("GoogleAuthSettings").GetValue<string>("ClientId");
        googleOptions.ClientSecret = builder.Configuration.GetSection("GoogleAuthSettings").GetValue<string>("ClientSecret");
        googleOptions.SignInScheme = IdentityConstants.ExternalScheme;
    }).AddVkontakte(vkOptions =>
    {
        vkOptions.ClientId = builder.Configuration.GetSection("VkontakteAuthSettings").GetValue<string>("ClientId");
        vkOptions.ClientSecret = builder.Configuration.GetSection("VkontakteAuthSettings").GetValue<string>("ClientSecret");
        vkOptions.SignInScheme = IdentityConstants.ExternalScheme;
        vkOptions.CallbackPath = "/Account/ExternalSignInResponse/";
    });

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(90);
    });
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
        options.HttpsPort = 443;
    });
}

builder.Services.AddSignalR();

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IReviewsRepository, ReviewsRepository>();
builder.Services.AddScoped<ITagsRepository, TagsRepository>();
builder.Services.AddScoped<ILikesRepository, LikesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IRatesRepository, RatesRepository>();
builder.Services.AddScoped<IIllustrationsRepository, IllustrationsRepository>();
builder.Services.AddScoped<ILeisuresRepository, LeisuresRepository>();
builder.Services.AddScoped<ICloudService, CloudinaryCloudService>();

builder.Services.AddSingleton<ISearchService, AlgoliaSearchService>();
builder.Services.AddSingleton<ISearchClient>(new SearchClient(
    builder.Configuration.GetSection("AlgoliaSearch").GetValue<string>("ApplicationId"),
    builder.Configuration.GetSection("AlgoliaSearch").GetValue<string>("ApiKey")));

using (var serviceProvider = builder.Services.BuildServiceProvider())
{
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    await new RolesInitializer(userManager, roleManager, configuration).InitializeAsync();
}

var app = builder.Build();

if (!builder.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CheckStatusMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<CommentsHub>("/Comments");
});

app.Run();
