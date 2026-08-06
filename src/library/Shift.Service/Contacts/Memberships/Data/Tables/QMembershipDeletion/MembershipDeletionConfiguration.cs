using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Directory;

public class MembershipDeletionConfiguration : IEntityTypeConfiguration<MembershipDeletionEntity>
{
    public void Configure(EntityTypeBuilder<MembershipDeletionEntity> builder)
    {
        builder.ToTable("QMembershipDeletion", "contacts");
        builder.HasKey(x => new { x.DeletionIdentifier });
    }
}