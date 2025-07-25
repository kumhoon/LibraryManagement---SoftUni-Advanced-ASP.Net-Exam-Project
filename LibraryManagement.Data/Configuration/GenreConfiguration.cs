﻿namespace LibraryManagement.Data.Configuration
{
    using LibraryManagement.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using static LibraryManagement.Data.Common.DbEntityValidationConstants.GenreConstants;

    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> entity)
        {
            entity
                .HasKey(g => g.Id);

            entity
                .Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(GenreNameMaxLength);
        }
    }
}
