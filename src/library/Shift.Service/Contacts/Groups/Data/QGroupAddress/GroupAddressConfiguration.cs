using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Directory;

public class GroupAddressConfiguration : IEntityTypeConfiguration<GroupAddressEntity>
{
    public void Configure(EntityTypeBuilder<GroupAddressEntity> builder)
    {
        builder.ToTable("QGroupAddress", "contacts");
        builder.HasKey(x => x.AddressIdentifier);

        builder.Property(x => x.AddressIdentifier).HasColumnName("AddressIdentifier").IsRequired().HasDefaultValueSql("newid()");
        builder.Property(x => x.GroupIdentifier).HasColumnName("GroupIdentifier").IsRequired();
        builder.Property(x => x.AddressType).HasColumnName("AddressType").IsUnicode(false).HasMaxLength(50).IsRequired();
        builder.Property(x => x.City).HasColumnName("City").IsUnicode(false).HasMaxLength(128);
        builder.Property(x => x.Country).HasColumnName("Country").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.Description).HasColumnName("Description").IsUnicode(false).HasMaxLength(128);
        builder.Property(x => x.PostalCode).HasColumnName("PostalCode").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.Province).HasColumnName("Province").IsUnicode(false).HasMaxLength(64);
        builder.Property(x => x.Street1).HasColumnName("Street1").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.Street2).HasColumnName("Street2").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.Latitude).HasColumnName("Latitude").HasColumnType("decimal(6, 3)");
        builder.Property(x => x.Longitude).HasColumnName("Longitude").HasColumnType("decimal(6, 3)");
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");

        builder.HasOne(x => x.Group)
            .WithMany(x => x.Addresses)
            .HasForeignKey(x => x.GroupIdentifier)
            .HasPrincipalKey(x => x.GroupIdentifier);
    }
}
