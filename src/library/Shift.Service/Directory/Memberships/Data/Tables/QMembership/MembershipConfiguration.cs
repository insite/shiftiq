using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Directory;

public class MembershipConfiguration : IEntityTypeConfiguration<MembershipEntity>
{
    public void Configure(EntityTypeBuilder<MembershipEntity> builder)
    {
        builder.ToTable("QMembership", "contacts");
        builder.HasKey(x => new { x.MembershipIdentifier });

        builder.Property(x => x.MembershipIdentifier).HasColumnName("MembershipIdentifier").IsRequired();
        builder.Property(x => x.MembershipEffective).HasColumnName("MembershipEffective").IsRequired();
        builder.Property(x => x.MembershipFunction).HasColumnName("MembershipFunction").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.GroupIdentifier).HasColumnName("GroupIdentifier").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.UserIdentifier).HasColumnName("UserIdentifier").IsRequired();
        builder.Property(x => x.MembershipExpiry).HasColumnName("MembershipExpiry");
        builder.Property(x => x.Modified).HasColumnName("Modified").IsRequired();
        builder.Property(x => x.ModifiedBy).HasColumnName("ModifiedBy").IsRequired();

    }
}