using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Directory;

public class QPersonSecretConfiguration : IEntityTypeConfiguration<QPersonSecretEntity>
{
    public void Configure(EntityTypeBuilder<QPersonSecretEntity> builder) 
    {
        builder.ToTable("QPersonSecret", "contacts");
        builder.HasKey(x => new { x.SecretIdentifier });
            
            builder.Property(x => x.PersonIdentifier).HasColumnName("PersonIdentifier").IsRequired();
            builder.Property(x => x.SecretIdentifier).HasColumnName("SecretIdentifier").IsRequired();
            builder.Property(x => x.SecretType).HasColumnName("SecretType").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(x => x.SecretName).HasColumnName("SecretName").IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(x => x.SecretExpiry).HasColumnName("SecretExpiry").IsRequired();
            builder.Property(x => x.SecretLifetimeLimit).HasColumnName("SecretLifetimeLimit");
            builder.Property(x => x.SecretValue).HasColumnName("SecretValue").IsRequired().IsUnicode(false).HasMaxLength(100);

    }
}