using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Entities;

namespace MusicStore.DataAccess.Configurations;

public class ConcertConfiguration :IEntityTypeConfiguration<Concert>
{
    public void Configure(EntityTypeBuilder<Concert> builder)
    {
        builder.Property(p => p.Title)
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.Place)
            .HasMaxLength(100);

        builder.Property(p => p.ImageUrl)
            .IsUnicode(false)
            .HasMaxLength(1000);

        builder.Property(p => p.UnitPrice)
            .HasPrecision(11, 2);
    }
}