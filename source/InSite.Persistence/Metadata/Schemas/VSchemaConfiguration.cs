using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VSchemaConfiguration : EntityTypeConfiguration<VSchema>
    {
        public VSchemaConfiguration() : this("databases") { }

        public VSchemaConfiguration(string schema)
        {
            ToTable(schema + ".VSchema");
            HasKey(x => x.SchemaId);
        
            Property(x => x.ObjectCount).IsOptional();
            Property(x => x.SchemaId).IsRequired();
            Property(x => x.SchemaName).IsRequired().IsUnicode(true).HasMaxLength(128);
            Property(x => x.TableCount).IsOptional();
        }
    }
}
