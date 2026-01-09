using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class DivisionConfiguration : EntityTypeConfiguration<Division>
    {
        public DivisionConfiguration() : this("identities") { }

        public DivisionConfiguration(string schema)
        {
            ToTable(schema + ".Division");
            HasKey(x => new { x.DivisionIdentifier });

            Property(x => x.GroupCreated).IsRequired();
            Property(x => x.DivisionCode).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.DivisionDescription).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.DivisionIdentifier).IsRequired();
            Property(x => x.DivisionIdentifier).IsRequired();
            Property(x => x.DivisionName).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LastChangeTime).IsRequired();
            Property(x => x.LastChangeUser).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
        }
    }
}
