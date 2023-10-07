using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Entities;

namespace MusicStore.DataAccess.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(p => p.Email)
            .HasMaxLength(200)
            .IsRequired()
            .IsUnicode(false);
        
        builder.Property(p => p.FullName)
            .HasMaxLength(200);
    }
}