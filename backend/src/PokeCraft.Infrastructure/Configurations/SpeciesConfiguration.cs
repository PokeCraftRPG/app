using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeCraft.Domain;
using PokeCraft.Domain.Speciez;
using PokeCraft.Infrastructure.Entities;

namespace PokeCraft.Infrastructure.Configurations;

internal class SpeciesConfiguration : AggregateConfiguration<SpeciesEntity>
{
  public override void Configure(EntityTypeBuilder<SpeciesEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(PokemonDb.Species.Table.Table ?? string.Empty, PokemonDb.Species.Table.Schema);
    builder.HasKey(x => x.SpeciesId);

    builder.HasIndex(x => new { x.WorldId, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldUid, x.Id }).IsUnique();
    builder.HasIndex(x => new { x.WorldId, x.Number }).IsUnique();
    builder.HasIndex(x => new { x.WorldUid, x.Number }).IsUnique();
    builder.HasIndex(x => x.Category);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => new { x.WorldId, x.UniqueNameNormalized }).IsUnique();
    builder.HasIndex(x => new { x.WorldUid, x.UniqueNameNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.BaseFriendship);
    builder.HasIndex(x => x.CatchRate);
    builder.HasIndex(x => x.GrowthRate);

    builder.Property(x => x.Category).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<SpeciesCategory>());
    builder.Property(x => x.UniqueName).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.GrowthRate).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<GrowthRate>());
    builder.Property(x => x.Link).HasMaxLength(Url.MaximumLength);

    builder.HasOne(x => x.World).WithMany(x => x.Species)
      .HasPrincipalKey(x => x.WorldId).HasForeignKey(x => x.WorldId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
