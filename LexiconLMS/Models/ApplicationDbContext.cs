using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace LexiconLMS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Message> Messages { get; set; }

        //public DbSet<CourseDocument> CourseDocuments { get; set; }
        //public DbSet<ActivityDocument> ActivityDocuments { get; set; }
        //public DbSet<ModuleDocument> ModuleDocuments { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DocumentType>()
                .HasMany(dt => dt.CanView)
                .WithMany()
                .Map(dtr =>
                {
                    dtr.MapLeftKey("DocumentTypeId");
                    dtr.MapRightKey("IdentityRoleId");
                    dtr.ToTable("DocumentTypeCanView");
                });
            modelBuilder.Entity<DocumentType>()
                .HasMany(dt => dt.CanCreate)
                .WithMany()
                .Map(dtr =>
                {
                    dtr.MapLeftKey("DocumentTypeId");
                    dtr.MapRightKey("IdentityRoleId");
                    dtr.ToTable("DocumentTypeCanCreate");
                });
            modelBuilder.Entity<Message>()
                .HasOptional(m => m.Attachment)
                .WithRequired(a => a.Message);
            modelBuilder.Entity<Message>()
                .HasRequired(m => m.User)
                .WithMany(u => u.Messages);
            modelBuilder.Entity<Message>()
                .HasRequired(m => m.Chat)
                .WithMany(c => c.Messages);
            modelBuilder.Entity<Message>()
                .HasKey(m => m.Id);
            modelBuilder.Entity<Attachment>()
                .HasKey(a => a.MessageId);
            modelBuilder.Entity<Chat>()
                .HasMany(c => c.Messages)
                .WithRequired(m => m.Chat);
            modelBuilder.Entity<Chat>()
                .HasMany(c => c.Users)
                .WithMany(u => u.Chats);
            modelBuilder.Entity<Chat>()
                .HasRequired(c => c.Course);
            //    /*
            //    modelBuilder.Entity<Document>()
            //        .HasOptional(d => d.Module)
            //        .WithMany(m => m.Documents)
            //        .HasForeignKey(d => d.ModuleId)
            //        .WillCascadeOnDelete(true);
            //        */
            }


            //public System.Data.Entity.DbSet<LexiconLMS.Models.ApplicationUser> ApplicationUsers { get; set; }
        }
}