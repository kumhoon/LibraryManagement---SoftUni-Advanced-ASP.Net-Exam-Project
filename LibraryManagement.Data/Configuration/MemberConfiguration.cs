using LibraryManagement.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static LibraryManagement.Data.Common.DbEntityValidationConstants.MemberConstants;
namespace LibraryManagement.Data.Configuration
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> entity)
        {
           entity
                .HasKey(m => m.Id);

            entity
                .Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(MemberNameMaxLength);

            entity
                .Property(m => m.JoinDate)
                .IsRequired();

            entity
                .HasOne(m => m.User)
                .WithOne()
                .HasForeignKey<Member>(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
