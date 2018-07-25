﻿using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace LexiconLMS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Document> Documents { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false) {
        }

        public static ApplicationDbContext Create() {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DocumentType>()
                .HasMany(dt => dt.CanView)
                .WithMany()
                .Map(dtr => {
                    dtr.MapLeftKey("DocumentTypeId");
                    dtr.MapRightKey("IdentityRoleId");
                    dtr.ToTable("DocumentTypeCanView");
                });
            modelBuilder.Entity<DocumentType>()
                .HasMany(dt => dt.CanCreate)
                .WithMany()
                .Map(dtr => {
                    dtr.MapLeftKey("DocumentTypeId");
                    dtr.MapRightKey("IdentityRoleId");
                    dtr.ToTable("DocumentTypeCanCreate");
                });
        }
        public System.Data.Entity.DbSet<LexiconLMS.Models.Activity> Activities { get; set; }

        //public System.Data.Entity.DbSet<LexiconLMS.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}