using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChildCareSystem.Areas.Identity.Data;
using ChildCareSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChildCareSystem.Data
{
    public class ChildCareSystemContext : IdentityDbContext<ChildCareSystemUser>
    {
        public ChildCareSystemContext(DbContextOptions<ChildCareSystemContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ChildCareSystemUser>(entity =>
            {
                entity.ToTable(name: "Users");
            });
            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Roles");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable(name: "UserRoles");
            });
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable(name: "UserClaims");
            });
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable(name: "UserLogins");
            });
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable(name: "RoleClaims");
            });
            builder.Entity<Service>()
                .HasIndex(s => s.ServiceName).IsUnique();

        }

        public DbSet<Blog> Blog { get; set; }
        public DbSet<BlogCategory> BlogCategory { get; set; }
        public DbSet<Specialty> Specialty { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Patient> Patient { get; set; }

    }
}
