using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VViewConfiguration : EntityTypeConfiguration<VView>
    {
        public VViewConfiguration() : this("databases") { }

        public VViewConfiguration(string schema)
        {
            ToTable(schema + ".VView");
            HasKey(x => x.ViewId);
        
            Property(x => x.ColumnCount).IsRequired();
            Property(x => x.SchemaId).IsRequired();
            Property(x => x.SchemaName).IsRequired().IsUnicode(true).HasMaxLength(128);
            Property(x => x.ViewId).IsRequired();
            Property(x => x.ViewName).IsRequired().IsUnicode(true).HasMaxLength(128);
        }
    }
}
