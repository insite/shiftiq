using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class VViewColumnConfiguration : EntityTypeConfiguration<VViewColumn>
    {
        public VViewColumnConfiguration() : this("databases") { }

        public VViewColumnConfiguration(string schema)
        {
            ToTable(schema + ".VViewColumn");
            HasKey(x => new { x.SchemaName, x.ViewName, x.ColumnName });

            Property(x => x.ColumnName).IsRequired().IsUnicode(true).HasMaxLength(128);
            Property(x => x.DataType).IsRequired().IsUnicode(true).HasMaxLength(128);
            Property(x => x.IsIdentity).IsOptional();
            Property(x => x.IsRequired).IsOptional();
            Property(x => x.MaximumLength).IsOptional();
            Property(x => x.OrdinalPosition).IsOptional();
            Property(x => x.SchemaId).IsRequired();
            Property(x => x.SchemaName).IsRequired().IsUnicode(true).HasMaxLength(128);
            Property(x => x.ViewId).IsRequired();
            Property(x => x.ViewName).IsRequired().IsUnicode(true).HasMaxLength(128);
        }
    }
}
