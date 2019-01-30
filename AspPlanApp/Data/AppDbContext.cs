﻿using AspPlanApp.Models.DbModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AspPlanApp.Models
{
    public class AppDbContext : IdentityDbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<DbModels.User> User { get; set; }
        public DbSet<DbModels.Org> Org { get; set; }
        public DbSet<DbModels.OrgStaff> OrgStaff { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<IdentityUser>().ToTable("User");
            builder.Entity<User>().ToTable("User");
            builder.Entity<IdentityRole>().ToTable("Role");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin"); 
            builder.Entity<IdentityUserToken<string>>().ToTable("UserToken"); 
            builder.Entity<IdentityRoleClaim<string>>().ToTable("UserRoleClaim");

            builder.Entity<DbModels.Org>().HasKey(k => k.orgId);
        }
    }
}