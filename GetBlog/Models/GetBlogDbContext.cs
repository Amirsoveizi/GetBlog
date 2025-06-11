// /Models/GetBlogDbContext.cs

// --- REQUIRED using statements ---
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

// --- Your project's models namespace ---
namespace GetBlog.Models
{
    /// <summary>
    /// This is the primary database context for the application, integrated with ASP.NET Identity.
    /// It inherits from IdentityDbContext to automatically handle User and Role tables.
    /// </summary>
    public class GetBlogDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Constructor that points to the connection string named "GetBlog" in your Web.config file.
        /// </summary>
        public GetBlogDbContext() : base("name=GetBlog", throwIfV1Schema: false)
        {
        }

        // --- Your Custom Application Models ---
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Media> MediaItems { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AuthorProfile> AuthorProfiles { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Setting> Settings { get; set; }

        // NOTE: DO NOT add DbSet for ApplicationUser or IdentityRole.
        // They are already included in the base IdentityDbContext class.

        /// <summary>
        /// A factory method required by the Owin middleware to create an instance of the DbContext.
        /// </summary>
        public static GetBlogDbContext Create()
        {
            return new GetBlogDbContext();
        }

        /// <summary>
        /// This method is used to configure the database model using the Fluent API.
        /// It's where you define complex relationships and constraints.
        /// </summary>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // VERY IMPORTANT: This line MUST be the first call in this method.
            // It allows the base IdentityDbContext to configure its own required tables
            // (AspNetUsers, AspNetRoles, AspNetUserRoles, etc.) correctly.
            base.OnModelCreating(modelBuilder);

            // --- Custom Relationship Configurations ---

            // Prevents cascade delete errors when a User is deleted.
            modelBuilder.Entity<Comment>()
                .HasRequired(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .WillCascadeOnDelete(false);

            // Prevents cascade delete errors when a User is deleted.
            modelBuilder.Entity<Article>()
                .HasRequired(a => a.Author)
                .WithMany(u => u.Articles)
                .HasForeignKey(a => a.AuthorId)
                .WillCascadeOnDelete(false);

            // Prevents cascade delete errors when a User is deleted.
            modelBuilder.Entity<Media>()
                .HasRequired(m => m.Uploader)
                .WithMany() // Configured for a one-way navigation (no Media collection on User)
                .HasForeignKey(m => m.UploaderId)
                .WillCascadeOnDelete(false);

            // Prevents cascade delete errors when a User is deleted.
            modelBuilder.Entity<AuditLog>()
                .HasOptional(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .WillCascadeOnDelete(false);

            // NO other configurations are needed for Identity itself.
            // DO NOT call base.OnModelCreating() a second time at the end.
        }
    }
}