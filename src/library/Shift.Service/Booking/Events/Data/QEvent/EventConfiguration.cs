using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Booking;

public class EventConfiguration : IEntityTypeConfiguration<EventEntity>
{
    public void Configure(EntityTypeBuilder<EventEntity> builder)
    {
        builder.ToTable("QEvent", "events");
        builder.HasKey(x => new { x.EventIdentifier });

        builder.Property(x => x.EventBillingType).HasColumnName("EventBillingType").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.EventClassCode).HasColumnName("EventClassCode").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.ExamDurationInMinutes).HasColumnName("ExamDurationInMinutes");
        builder.Property(x => x.EventFormat).HasColumnName("EventFormat").IsUnicode(false).HasMaxLength(10);
        builder.Property(x => x.EventIdentifier).HasColumnName("EventIdentifier").IsRequired();
        builder.Property(x => x.EventNumber).HasColumnName("EventNumber").IsRequired();
        builder.Property(x => x.EventSchedulingStatus).HasColumnName("EventSchedulingStatus").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.EventTitle).HasColumnName("EventTitle").IsUnicode(false).HasMaxLength(400);
        builder.Property(x => x.EventType).HasColumnName("EventType").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.DistributionCode).HasColumnName("DistributionCode").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.DistributionErrors).HasColumnName("DistributionErrors").IsUnicode(false);
        builder.Property(x => x.DistributionExpected).HasColumnName("DistributionExpected");
        builder.Property(x => x.DistributionProcess).HasColumnName("DistributionProcess").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.DistributionOrdered).HasColumnName("DistributionOrdered");
        builder.Property(x => x.DistributionShipped).HasColumnName("DistributionShipped");
        builder.Property(x => x.DistributionStatus).HasColumnName("DistributionStatus").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.DistributionTracked).HasColumnName("DistributionTracked");
        builder.Property(x => x.ExamStarted).HasColumnName("ExamStarted");
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.VenueLocationIdentifier).HasColumnName("VenueLocationIdentifier");
        builder.Property(x => x.VenueRoom).HasColumnName("VenueRoom").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.PublicationErrors).HasColumnName("PublicationErrors").IsUnicode(false);
        builder.Property(x => x.VenueCoordinatorIdentifier).HasColumnName("VenueCoordinatorIdentifier");
        builder.Property(x => x.EventDescription).HasColumnName("EventDescription").IsUnicode(false);
        builder.Property(x => x.LastChangeTime).HasColumnName("LastChangeTime");
        builder.Property(x => x.LastChangeType).HasColumnName("LastChangeType").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.LastChangeUser).HasColumnName("LastChangeUser").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.EventScheduledStart).HasColumnName("EventScheduledStart").IsRequired();
        builder.Property(x => x.EventScheduledEnd).HasColumnName("EventScheduledEnd");
        builder.Property(x => x.DurationQuantity).HasColumnName("DurationQuantity");
        builder.Property(x => x.DurationUnit).HasColumnName("DurationUnit").IsUnicode(false).HasMaxLength(10);
        builder.Property(x => x.EventSource).HasColumnName("EventSource").IsUnicode(false);
        builder.Property(x => x.CreditHours).HasColumnName("CreditHours").HasPrecision(5, 2);
        builder.Property(x => x.CapacityMinimum).HasColumnName("CapacityMinimum");
        builder.Property(x => x.CapacityMaximum).HasColumnName("CapacityMaximum");
        builder.Property(x => x.ExamType).HasColumnName("ExamType").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.AchievementIdentifier).HasColumnName("AchievementIdentifier");
        builder.Property(x => x.RegistrationDeadline).HasColumnName("RegistrationDeadline");
        builder.Property(x => x.EventSummary).HasColumnName("EventSummary").IsUnicode(false);
        builder.Property(x => x.EventPublicationStatus).HasColumnName("EventPublicationStatus").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.Content).HasColumnName("Content").IsUnicode(true);
        builder.Property(x => x.WaitlistEnabled).HasColumnName("WaitlistEnabled").IsRequired();
        builder.Property(x => x.RegistrationStart).HasColumnName("RegistrationStart");
        builder.Property(x => x.EventRequisitionStatus).HasColumnName("EventRequisitionStatus").IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.InvigilatorMinimum).HasColumnName("InvigilatorMinimum");
        builder.Property(x => x.ExamMaterialReturnShipmentCode).HasColumnName("ExamMaterialReturnShipmentCode").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.ExamMaterialReturnShipmentReceived).HasColumnName("ExamMaterialReturnShipmentReceived");
        builder.Property(x => x.ExamMaterialReturnShipmentCondition).HasColumnName("ExamMaterialReturnShipmentCondition").IsUnicode(false).HasMaxLength(10);
        builder.Property(x => x.IntegrationWithholdGrades).HasColumnName("IntegrationWithholdGrades").IsRequired();
        builder.Property(x => x.IntegrationWithholdDistribution).HasColumnName("IntegrationWithholdDistribution").IsRequired();
        builder.Property(x => x.VenueOfficeIdentifier).HasColumnName("VenueOfficeIdentifier");
        builder.Property(x => x.AppointmentType).HasColumnName("AppointmentType").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.RegistrationLocked).HasColumnName("RegistrationLocked");
        builder.Property(x => x.AllowRegistrationWithLink).HasColumnName("AllowRegistrationWithLink");
        builder.Property(x => x.LearnerRegistrationGroupIdentifier).HasColumnName("LearnerRegistrationGroupIdentifier");
        builder.Property(x => x.PersonCodeIsRequired).HasColumnName("PersonCodeIsRequired").IsRequired();
        builder.Property(x => x.AllowMultipleRegistrations).HasColumnName("AllowMultipleRegistrations").IsRequired();
        builder.Property(x => x.EventCalendarColor).HasColumnName("EventCalendarColor").IsUnicode(false).HasMaxLength(7);
        builder.Property(x => x.RegistrationFields).HasColumnName("RegistrationFields").IsUnicode(false);
        builder.Property(x => x.MandatorySurveyFormIdentifier).HasColumnName("MandatorySurveyFormIdentifier");

    }
}