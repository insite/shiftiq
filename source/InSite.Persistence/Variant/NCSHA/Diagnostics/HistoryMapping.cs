using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class HistoryMapping : EntityTypeConfiguration<History>
    {
        public HistoryMapping() : this("custom_ncsha")
        {
        }

        public HistoryMapping(string schema)
        {
            ToTable(schema + ".History");
            HasKey(x => new { x.RecordId });

            Property(x => x.RecordId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.RecordTime).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);
            Property(x => x.UserName).IsRequired().IsUnicode(false).HasMaxLength(128);
            Property(x => x.UserEmail).IsRequired().IsUnicode(false).HasMaxLength(40);
            Property(x => x.EventType).IsRequired().IsUnicode(false).HasMaxLength(70);
        }
    }
}
