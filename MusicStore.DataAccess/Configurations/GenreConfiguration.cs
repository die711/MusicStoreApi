using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Entities;

namespace MusicStore.DataAccess.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(50);


        builder.HasData(new List<Genre>
        {
            new(){Id = 1, Name = "Rock"},
            new(){Id = 2, Name = "Pop"},
            new(){Id = 3, Name = "Salsa"},
            new(){Id = 4, Name = "K-Pop"},
            new(){Id = 5, Name = "Blues"}
        });



    }
}