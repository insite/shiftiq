using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Directory;

public class GroupConfiguration : IEntityTypeConfiguration<GroupEntity>
{
    public void Configure(EntityTypeBuilder<GroupEntity> builder)
    {
        builder.ToTable("QGroup", "contacts");
        builder.HasKey(x => new { x.GroupIdentifier });

        builder.Property(x => x.GroupIdentifier).HasColumnName("GroupIdentifier").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.ParentGroupIdentifier).HasColumnName("ParentGroupIdentifier");
        builder.Property(x => x.SurveyFormIdentifier).HasColumnName("SurveyFormIdentifier");
        builder.Property(x => x.MessageToUserWhenMembershipStarted).HasColumnName("MessageToUserWhenMembershipStarted");
        builder.Property(x => x.MessageToAdminWhenMembershipStarted).HasColumnName("MessageToAdminWhenMembershipStarted");
        builder.Property(x => x.MessageToAdminWhenEventVenueChanged).HasColumnName("MessageToAdminWhenEventVenueChanged");
        builder.Property(x => x.AddNewUsersAutomatically).HasColumnName("AddNewUsersAutomatically").IsRequired();
        builder.Property(x => x.AllowSelfSubscription).HasColumnName("AllowSelfSubscription").IsRequired();
        builder.Property(x => x.GroupCapacity).HasColumnName("GroupCapacity");
        builder.Property(x => x.GroupCategory).HasColumnName("GroupCategory").IsUnicode(false).HasMaxLength(120);
        builder.Property(x => x.GroupCode).HasColumnName("GroupCode").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.GroupCreated).HasColumnName("GroupCreated").IsRequired();
        builder.Property(x => x.GroupType).HasColumnName("GroupType").IsRequired().IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.GroupDescription).HasColumnName("GroupDescription").IsUnicode(false);
        builder.Property(x => x.GroupFax).HasColumnName("GroupFax").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.GroupImage).HasColumnName("GroupImage").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.GroupIndustry).HasColumnName("GroupIndustry").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.GroupIndustryComment).HasColumnName("GroupIndustryComment").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.GroupLabel).HasColumnName("GroupLabel").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.GroupName).HasColumnName("GroupName").IsRequired().IsUnicode(false).HasMaxLength(90);
        builder.Property(x => x.GroupOffice).HasColumnName("GroupOffice").IsUnicode(false).HasMaxLength(800);
        builder.Property(x => x.GroupPhone).HasColumnName("GroupPhone").IsUnicode(false).HasMaxLength(32);
        builder.Property(x => x.GroupRegion).HasColumnName("GroupRegion").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.GroupSize).HasColumnName("GroupSize").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.GroupWebSiteUrl).HasColumnName("GroupWebSiteUrl").IsUnicode(false).HasMaxLength(500);
        builder.Property(x => x.ShippingPreference).HasColumnName("ShippingPreference").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.SurveyNecessity).HasColumnName("SurveyNecessity").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.LastChangeTime).HasColumnName("LastChangeTime").IsRequired();
        builder.Property(x => x.LastChangeType).HasColumnName("LastChangeType").IsRequired().IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.LastChangeUser).HasColumnName("LastChangeUser").IsRequired().IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.GroupEmail).HasColumnName("GroupEmail").IsUnicode(false).HasMaxLength(254);
        builder.Property(x => x.SocialMediaUrls).HasColumnName("SocialMediaUrls").IsUnicode(false);
        builder.Property(x => x.GroupExpiry).HasColumnName("GroupExpiry");
        builder.Property(x => x.LifetimeUnit).HasColumnName("LifetimeUnit").IsUnicode(false).HasMaxLength(6);
        builder.Property(x => x.LifetimeQuantity).HasColumnName("LifetimeQuantity");
        builder.Property(x => x.MessageToAdminWhenMembershipEnded).HasColumnName("MessageToAdminWhenMembershipEnded");
        builder.Property(x => x.MessageToUserWhenMembershipEnded).HasColumnName("MessageToUserWhenMembershipEnded");
        builder.Property(x => x.MembershipProductIdentifier).HasColumnName("MembershipProductIdentifier");
        builder.Property(x => x.AllowJoinGroupUsingLink).HasColumnName("AllowJoinGroupUsingLink").IsRequired();
    }
}