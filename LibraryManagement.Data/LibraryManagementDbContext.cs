namespace LibraryManagement.Data
{
    using LibraryManagement.Data.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System.Reflection;

    public class LibraryManagementDbContext : IdentityDbContext
    {
        public LibraryManagementDbContext(DbContextOptions<LibraryManagementDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<Book> Books { get; set; }

        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<BorrowingRecord> BorrowingRecords { get; set; }

        public virtual DbSet<Genre> Genres { get; set; }

        public virtual DbSet<Member> Memberships { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
