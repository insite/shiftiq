using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class TProgramContainmentConfiguration : EntityTypeConfiguration<TProgramContainment>
    {
        public TProgramContainmentConfiguration() : this("records") { }

        public TProgramContainmentConfiguration(string schema)
        {
            ToTable(schema + ".TProgramContainment");
            HasKey(x => new { x.ParentProgramIdentifier, x.ChildProgramIdentifier });

            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.ChildSequence).IsRequired();
            Property(x => x.Created).IsRequired();
            Property(x => x.CreatedBy).IsRequired();
        }
    }
}
