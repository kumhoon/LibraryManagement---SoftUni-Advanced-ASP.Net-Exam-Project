using LibraryManagement.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static LibraryManagement.Data.Common.DbEntityValidationConstants.Book;

namespace LibraryManagement.Data.Configuration
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> entity)
        {
            entity
                .HasKey(b => b.Id);

            entity
                .Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(BookTitleMaxLength);

            entity
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(b => b.Genre)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .Property(b => b.PublishedDate)
                .IsRequired();

            entity
                .Property(b => b.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            entity
                .HasQueryFilter(b => b.IsDeleted == false);

            entity
                .HasOne(b => b.BookCreator)
                .WithMany()
                .HasForeignKey(b => b.BookCreatorId)    
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
