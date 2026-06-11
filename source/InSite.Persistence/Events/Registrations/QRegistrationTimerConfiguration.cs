using System.Data.Entity.ModelConfiguration;

using InSite.Application.Registrations.Read;

namespace InSite.Persistence
{
    public class QRegistrationTimerConfiguration : EntityTypeConfiguration<QRegistrationTimer>
    {
        public QRegistrationTimerConfiguration() : this("registrations") { }

        public QRegistrationTimerConfiguration(string schema)
        {
            ToTable(schema + ".QRegistrationTimer");
            HasKey(x => new { x.TriggerCommand });

            Property(x => x.RegistrationIdentifier).IsRequired();
            Property(x => x.TimerDescription).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.TimerStatus).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.TriggerCommand).IsRequired();
            Property(x => x.TriggerTime).IsRequired();
        }
    }
}
