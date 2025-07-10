namespace LibraryManagement.Data.Configuration
{
    using LibraryManagement.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class BorrowingRecordConfiguration : IEntityTypeConfiguration<BorrowingRecord>
    {
        public void Configure(EntityTypeBuilder<BorrowingRecord> entity)
        {
            entity
                .HasKey(br => br.Id);

            entity
                .HasOne(br => br.Member)
                .WithMany(m => m.BorrowingRecords)
                .HasForeignKey(br => br.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(br => br.Book)
                .WithMany()
                .HasForeignKey(br => br.BookId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .Property(br => br.BorrowDate)
                .IsRequired();

            entity
                .Property(br => br.ReturnDate)
                .IsRequired(false);
        }
    }
}
