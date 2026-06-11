using System.Data.Entity.ModelConfiguration;

using Shift.Common.Timeline.Changes;

namespace InSite.Persistence
{
    public class AggregateConfiguration : EntityTypeConfiguration<SerializedAggregate>
    {
        public AggregateConfiguration() : this("logs") { }

        public AggregateConfiguration(string schema)
        {
            ToTable(schema + ".Aggregate");
            HasKey(x => new { x.AggregateIdentifier });

            Property(x => x.AggregateClass).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.AggregateExpires).IsOptional();
            Property(x => x.AggregateIdentifier).IsRequired();
            Property(x => x.AggregateType).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.OriginOrganization).IsRequired();
        }
    }
}
