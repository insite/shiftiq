using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VGroup2Configuration : EntityTypeConfiguration<VGroupDetail>
    {
        public VGroup2Configuration() : this("contacts") { }

        public VGroup2Configuration(string schema)
        {
            ToTable(schema + ".VGroupDetail");
            HasKey(x => x.GroupIdentifier);

            HasRequired(x => x.Organization).WithMany(x => x.Groups).HasForeignKey(x => x.OrganizationIdentifier);
            HasOptional(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentGroupIdentifier);
        }
    }
}
