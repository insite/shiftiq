using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Security;

public class UserConnectionConfiguration : IEntityTypeConfiguration<UserConnectionEntity>
{
    public void Configure(EntityTypeBuilder<UserConnectionEntity> builder)
    {
        builder.ToTable("QUserConnection", "identities");
        builder.HasKey(x => new { x.FromUserIdentifier, x.ToUserIdentifier });

        builder.Property(x => x.Connected).HasColumnName("Connected").IsRequired();
        builder.Property(x => x.IsManager).HasColumnName("IsManager").IsRequired();
        builder.Property(x => x.IsSupervisor).HasColumnName("IsSupervisor").IsRequired();
        builder.Property(x => x.IsValidator).HasColumnName("IsValidator").IsRequired();
        builder.Property(x => x.FromUserIdentifier).HasColumnName("FromUserIdentifier").IsRequired();
        builder.Property(x => x.ToUserIdentifier).HasColumnName("ToUserIdentifier").IsRequired();
        builder.Property(x => x.IsLeader).HasColumnName("IsLeader").IsRequired();

    }
}