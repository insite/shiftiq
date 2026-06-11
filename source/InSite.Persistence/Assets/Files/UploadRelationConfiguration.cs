using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class UploadRelationConfiguration : EntityTypeConfiguration<UploadRelation>
    {
        public UploadRelationConfiguration() : this("resources") { }

        public UploadRelationConfiguration(string schema)
        {
            ToTable(schema + ".UploadRelation");
            HasKey(x => new { x.ContainerIdentifier,x.UploadIdentifier });
            Property(x => x.ContainerIdentifier).IsRequired();
            Property(x => x.ContainerType).IsRequired().IsUnicode(false).HasMaxLength(32);
            Property(x => x.UploadIdentifier).IsRequired();
        }
    }
}
