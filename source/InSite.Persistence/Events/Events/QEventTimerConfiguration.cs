using System.Data.Entity.ModelConfiguration;

using InSite.Application.Events.Read;

namespace InSite.Persistence
{
    public class QEventTimerConfiguration : EntityTypeConfiguration<QEventTimer>
    {
        public QEventTimerConfiguration() : this("events") { }

        public QEventTimerConfiguration(string schema)
        {
            ToTable(schema + ".QEventTimer");
            HasKey(x => new { x.TriggerCommand });

            Property(x => x.EventIdentifier).IsRequired();
            Property(x => x.TimerDescription).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.TimerStatus).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.TriggerCommand).IsRequired();
            Property(x => x.TriggerTime).IsRequired();
        }
    }
}