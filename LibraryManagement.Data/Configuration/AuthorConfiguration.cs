﻿namespace LibraryManagement.Data.Configuration
{
    using LibraryManagement.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using static LibraryManagement.Data.Common.DbEntityValidationConstants.AuthorConstants;
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> entity)
        {
            entity
                .HasKey(a => a.Id);

            entity
                .Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(AuthorNameMaxLength);

            entity
                .Property(a => a.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            entity
                .HasQueryFilter(a => a.IsDeleted == false);

            entity
                .Property(a => a.ImageUrl)
                .IsRequired(false);
        }
    }
}
