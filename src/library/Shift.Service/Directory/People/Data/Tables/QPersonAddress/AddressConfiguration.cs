using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Directory;

public class AddressConfiguration : IEntityTypeConfiguration<AddressEntity>
{
    public void Configure(EntityTypeBuilder<AddressEntity> builder)
    {
        builder.ToTable("QPersonAddress", "contacts");
        builder.HasKey(x => x.AddressIdentifier);

        builder.Property(x => x.AddressIdentifier).HasColumnName("AddressIdentifier").IsRequired();
        builder.Property(x => x.City).HasColumnName("City").IsUnicode(false).HasMaxLength(128);
        builder.Property(x => x.Country).HasColumnName("Country").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.Description).HasColumnName("Description").IsUnicode(false).HasMaxLength(128);
        builder.Property(x => x.PostalCode).HasColumnName("PostalCode").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.Province).HasColumnName("Province").IsUnicode(false).HasMaxLength(64);
        builder.Property(x => x.Street1).HasColumnName("Street1").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.Street2).HasColumnName("Street2").IsUnicode(false).HasMaxLength(200);
    }
}
