using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class TProgramCategoryConfiguration : EntityTypeConfiguration<TProgramCategory>
    {
        public TProgramCategoryConfiguration()
        {
            ToTable("TProgramCategory", "learning");
            HasKey(x => new { x.ProgramIdentifier, x.ItemIdentifier });

            Property(x => x.ProgramIdentifier).IsRequired();
            Property(x => x.ItemIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier);

            HasRequired(a => a.Category).WithMany(b => b.Programs).HasForeignKey(c => c.ItemIdentifier).WillCascadeOnDelete(false);
        }
    }
}
