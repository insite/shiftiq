using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    public class VGroupConfiguration : EntityTypeConfiguration<VGroup>
    {
        public VGroupConfiguration() : this("contacts") { }

        public VGroupConfiguration(string schema)
        {
            ToTable(schema + ".VGroup");
            HasKey(x => new { x.GroupIdentifier });

            Property(x => x.GroupCode).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.GroupIdentifier).IsRequired();
            Property(x => x.GroupName).IsRequired().IsUnicode(false).HasMaxLength(148);
            Property(x => x.GroupOffice).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.GroupPhone).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.GroupSize).IsOptional();
            Property(x => x.GroupType).IsOptional().IsUnicode(false).HasMaxLength(32);
            Property(x => x.ParentGroupIdentifier).IsOptional();
            Property(x => x.ParentGroupName).IsOptional().IsUnicode(false).HasMaxLength(148);
            Property(x => x.OrganizationIdentifier).IsRequired();
        }
    }
}