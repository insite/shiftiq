using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    internal class VCatalogProgramConfiguration : EntityTypeConfiguration<VCatalogProgram>
    {
        public VCatalogProgramConfiguration()
        {
            ToTable("VCatalogProgram", "records");
            HasKey(x => new { x.CatalogIdentifier, x.ProgramIdentifier, x.ProgramCategoryItemIdentifier });
        }
    }
}
