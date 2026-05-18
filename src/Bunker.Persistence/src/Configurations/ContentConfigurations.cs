using Bunker.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bunker.Persistence.Configurations;

public readonly record struct PersistenceConfigurations;

internal class BotPersonalityPresetConfiguration : IEntityTypeConfiguration<PersonalityPreset>
{
    public void Configure(EntityTypeBuilder<PersonalityPreset> builder)
    {
        builder.ToTable("PersonalityPresets", "content");
    
        builder.Property<int>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.HasAlternateKey(x => x.PublicId);

        builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(255).IsRequired();
    }
}

internal class CardPackConfiguration : IEntityTypeConfiguration<CardPack>
{
    public void Configure(EntityTypeBuilder<CardPack> builder)
    {
        builder.ToTable("CardPacks", "content");

        builder.Property<int>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");

        builder.HasAlternateKey(x => x.PublicId);

        builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(255).IsRequired();

        builder.OwnsMany(x => x.Cards, cb =>
        {
            cb.ToTable("CardPackCards", "content");
            
            cb.WithOwner()
                .HasPrincipalKey(x => x.PublicId)
                .HasForeignKey(x => x.CardPackId);

            cb.HasKey(x => new { x.CardPackId, x.CardId });

            cb.HasOne<Card>()
                .WithMany()
                .HasForeignKey(x => x.CardId)
                .HasPrincipalKey(x => x.PublicId);
        });
    }
}

internal class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("Cards", "content");

        builder.Property<int>("Id").ValueGeneratedOnAdd();
        builder.HasKey("Id");
        
        builder.HasAlternateKey(x => x.PublicId);

        builder.UseTptMappingStrategy();
    }
}

internal class ProfessionCardConfiguration : IEntityTypeConfiguration<ProfessionCard>
{
    public void Configure(EntityTypeBuilder<ProfessionCard> builder)
    {
        builder.ToTable("ProfessionCards", "content");
        builder.Property(x => x.Profession).HasColumnName("Profession");
    }
}

internal class HobbiesCardConfiguration : IEntityTypeConfiguration<HobbiesCard>
{
    public void Configure(EntityTypeBuilder<HobbiesCard> builder)
    {
        builder.ToTable("HobbiesCards", "content");
        builder.Property(x => x.Hobbies).HasColumnName("Hobbies");
    }
}

internal class AgeCardConfiguration : IEntityTypeConfiguration<AgeCard>
{
    public void Configure(EntityTypeBuilder<AgeCard> builder)
    {
        builder.ToTable("AgeCards", "content");
        builder.Property(x => x.Age).HasColumnName("Age");
    }
}

internal class SexCardConfiguration : IEntityTypeConfiguration<SexCard>
{
    public void Configure(EntityTypeBuilder<SexCard> builder)
    {
        builder.ToTable("SexCards", "content");
        builder.Property(x => x.Sex).HasColumnName("Sex");
    }
}

internal class FactCardConfiguration : IEntityTypeConfiguration<FactCard>
{
    public void Configure(EntityTypeBuilder<FactCard> builder)
    {
        builder.ToTable("FactCards", "content");
        builder.Property(x => x.Fact).HasColumnName("Fact");
    }
}
