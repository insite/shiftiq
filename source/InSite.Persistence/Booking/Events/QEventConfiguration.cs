using System.Data.Entity.ModelConfiguration;

using InSite.Application.Events.Read;

namespace InSite.Persistence
{
    public class QEventConfiguration : EntityTypeConfiguration<QEvent>
    {
        public QEventConfiguration() : this("events") { }

        public QEventConfiguration(string schema)
        {
            ToTable(schema + ".QEvent");
            HasKey(x => new { x.EventIdentifier });

            HasOptional(a => a.VenueCoordinator).WithMany(b => b.VenueCoordinatorEvents).HasForeignKey(c => new { c.OrganizationIdentifier, c.VenueCoordinatorIdentifier });
            HasOptional(a => a.VenueLocation).WithMany(b => b.VenueLocationEvents).HasForeignKey(c => c.VenueLocationIdentifier);
            HasOptional(a => a.VenueOffice).WithMany(b => b.VenueOfficeEvents).HasForeignKey(c => c.VenueOfficeIdentifier);
            HasOptional(a => a.Achievement).WithMany(b => b.Events).HasForeignKey(c => c.AchievementIdentifier);

            Property(x => x.AchievementIdentifier).IsOptional();
            Property(x => x.AllowRegistrationWithLink).IsOptional();
            Property(x => x.CapacityMaximum).IsOptional();
            Property(x => x.CapacityMinimum).IsOptional();
            Property(x => x.Content).IsOptional().IsUnicode(true);
            Property(x => x.CreditHours).IsOptional();
            Property(x => x.DistributionCode).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.DistributionErrors).IsOptional().IsUnicode(false);
            Property(x => x.DistributionExpected).IsOptional();
            Property(x => x.DistributionOrdered).IsOptional();
            Property(x => x.DistributionProcess).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.DistributionShipped).IsOptional();
            Property(x => x.DistributionStatus).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.DistributionTracked).IsOptional();
            Property(x => x.DurationQuantity).IsOptional();
            Property(x => x.DurationUnit).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.EventBillingType).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.EventClassCode).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.EventCalendarColor).IsOptional().IsUnicode(false).HasMaxLength(7);
            Property(x => x.EventDescription).IsOptional().IsUnicode(false);
            Property(x => x.EventFormat).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.EventIdentifier).IsRequired();
            Property(x => x.EventNumber).IsRequired();
            Property(x => x.EventPublicationStatus).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.EventRequisitionStatus).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.EventScheduledEnd).IsOptional();
            Property(x => x.EventScheduledStart).IsRequired();
            Property(x => x.EventSchedulingStatus).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.EventSource).IsOptional().IsUnicode(false);
            Property(x => x.EventSummary).IsOptional().IsUnicode(false);
            Property(x => x.EventTitle).IsOptional().IsUnicode(false).HasMaxLength(400);
            Property(x => x.EventType).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.ExamDurationInMinutes).IsOptional();
            Property(x => x.ExamMaterialReturnShipmentCode).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.ExamMaterialReturnShipmentCondition).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.ExamMaterialReturnShipmentReceived).IsOptional();
            Property(x => x.ExamStarted).IsOptional();
            Property(x => x.ExamType).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.IntegrationWithholdDistribution).IsRequired();
            Property(x => x.IntegrationWithholdGrades).IsRequired();
            Property(x => x.InvigilatorMinimum).IsOptional();
            Property(x => x.LastChangeTime).IsOptional();
            Property(x => x.LastChangeType).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LastChangeUser).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LearnerRegistrationGroupIdentifier).IsOptional();
            Property(x => x.PublicationErrors).IsOptional().IsUnicode(false);
            Property(x => x.RegistrationDeadline).IsOptional();
            Property(x => x.RegistrationStart).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.VenueCoordinatorIdentifier).IsOptional();
            Property(x => x.VenueLocationIdentifier).IsOptional();
            Property(x => x.VenueOfficeIdentifier).IsOptional();
            Property(x => x.VenueRoom).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.WaitlistEnabled).IsRequired();
            Property(x => x.RegistrationFields).IsUnicode(false);
        }
    }
}