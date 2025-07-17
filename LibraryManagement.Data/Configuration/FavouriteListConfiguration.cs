
namespace LibraryManagement.Data.Configuration
{
    using LibraryManagement.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class FavouriteListConfiguration : IEntityTypeConfiguration<FavouriteList>
    {
        public void Configure(EntityTypeBuilder<FavouriteList> entity)
        {
            entity.HasKey(f => new { f.MemberId, f.BookId });

            entity
                .HasOne(f => f.Member)
                .WithMany(m => m.Favourites)
                .HasForeignKey(f => f.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(f => f.Book)
                .WithMany(b => b.FavouritedBy)
                .HasForeignKey(f => f.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .Property(f => f.AddedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity
                .HasQueryFilter(f => f.Book.IsDeleted == false);
        }
    }
}
