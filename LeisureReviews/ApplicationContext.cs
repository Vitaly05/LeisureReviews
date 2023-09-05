using LeisureReviews.Models.Database;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LeisureReviews
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            :base(options) { }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Review>()
                .HasOne(r => r.Author)
                .WithMany(u => u.AuthoredReviews)
                .HasForeignKey(r => r.AuthorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Review>()
                .HasMany(r => r.LikedUsers)
                .WithMany(u => u.LikedReviews)
                .UsingEntity(j => j.ToTable("UserLikesReview"));

            base.OnModelCreating(builder);
        }
    }
}
