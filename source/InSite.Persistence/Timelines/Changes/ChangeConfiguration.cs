using System.Data.Entity.ModelConfiguration;

using Shift.Common.Timeline.Changes;

namespace InSite.Persistence
{
    public class ChangeConfiguration : EntityTypeConfiguration<SerializedChange>
    {
        public ChangeConfiguration() : this("logs") { }

        public ChangeConfiguration(string schema)
        {
            ToTable(schema + ".Change");
            HasKey(x => new { x.AggregateIdentifier, x.AggregateVersion });

            Ignore(x => x.AggregateState);

            Property(x => x.ChangeType).IsRequired().IsUnicode(false).HasMaxLength(50);
            Property(x => x.ChangeData).IsRequired().IsUnicode(true);
        }
    }
}
