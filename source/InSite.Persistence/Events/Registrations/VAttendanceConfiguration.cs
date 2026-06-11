using System.Data.Entity.ModelConfiguration;

using InSite.Application.Registrations.Read;

namespace InSite.Persistence
{
    public class VAttendanceConfiguration : EntityTypeConfiguration<VAttendance>
    {
        public VAttendanceConfiguration() : this("registrations") { }

        public VAttendanceConfiguration(string schema)
        {
            ToTable(schema + ".VAttendance");
            HasKey(x => x.RegistrationIdentifier);

            Property(x => x.AssessmentFormCode).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.AssessmentFormIdentifier).IsRequired();
            Property(x => x.AssessmentFormName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.AttendanceStatus).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.EventFormat).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.EventIdentifier).IsRequired();
            Property(x => x.EventNumber).IsRequired();
            Property(x => x.EventScheduledStart).IsRequired();
            Property(x => x.LastChangeTime).IsOptional();
            Property(x => x.LastChangeType).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LearnerCode).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.LearnerEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.LearnerIdentifier).IsRequired();
            Property(x => x.LearnerName).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.RegistrationIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
        }
    }
}