namespace LibraryManagement.Data.Configuration
{
    using LibraryManagement.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using static LibraryManagement.Data.Common.DbEntityValidationConstants.ReviewConstants;
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> entity)
        {
            entity
                .ToTable(tb => tb
                .HasCheckConstraint(
                    name: "CK_Review_Rating",
                    sql: "[Rating] >= 1 AND [Rating] <= 5"
                    )
                );
            entity
                .HasKey(r => r.Id);

            entity
                .Property(r => r.Rating)
                .IsRequired();

            entity
                .Property(r => r.Content)
                .IsRequired(false)
                .HasMaxLength(ReviewContentMaxLength);

            entity
                .Property(r => r.IsApproved)
                .HasDefaultValue(false);

            entity
                .HasOne(r => r.Member)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasIndex(r => new { r.MemberId, r.BookId })
                .IsUnique();

            entity
                .HasQueryFilter(r => r.Book.IsDeleted == false);

        }
    }
}
