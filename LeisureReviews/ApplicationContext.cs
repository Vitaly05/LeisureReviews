﻿using LeisureReviews.Models.Database;
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
    }
}
