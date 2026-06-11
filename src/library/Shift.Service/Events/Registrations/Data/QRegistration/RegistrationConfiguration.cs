using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Booking;

public class RegistrationConfiguration : IEntityTypeConfiguration<RegistrationEntity>
{
    public void Configure(EntityTypeBuilder<RegistrationEntity> builder) 
    {
        builder.ToTable("QRegistration", "registrations");
        builder.HasKey(x => new { x.RegistrationIdentifier });
            
        builder.Property(x => x.EventIdentifier).HasColumnName("EventIdentifier").IsRequired();
        builder.Property(x => x.EventPotentialConflicts).HasColumnName("EventPotentialConflicts").IsUnicode(false);
        builder.Property(x => x.ApprovalProcess).HasColumnName("ApprovalProcess").IsUnicode(false);
        builder.Property(x => x.ApprovalReason).HasColumnName("ApprovalReason").IsUnicode(false);
        builder.Property(x => x.ApprovalStatus).HasColumnName("ApprovalStatus").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.AttemptIdentifier).HasColumnName("AttemptIdentifier");
        builder.Property(x => x.AttendanceStatus).HasColumnName("AttendanceStatus").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.BillingCustomer).HasColumnName("BillingCustomer").IsUnicode(false).HasMaxLength(128);
        builder.Property(x => x.CandidateIdentifier).HasColumnName("CandidateIdentifier").IsRequired();
        builder.Property(x => x.CustomerIdentifier).HasColumnName("CustomerIdentifier");
        builder.Property(x => x.DistributionExpected).HasColumnName("DistributionExpected");
        builder.Property(x => x.EligibilityProcess).HasColumnName("EligibilityProcess").IsUnicode(false);
        builder.Property(x => x.EligibilityStatus).HasColumnName("EligibilityStatus").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.EmployerIdentifier).HasColumnName("EmployerIdentifier");
        builder.Property(x => x.ExamFormIdentifier).HasColumnName("ExamFormIdentifier");
        builder.Property(x => x.ExamTimeLimit).HasColumnName("ExamTimeLimit");
        builder.Property(x => x.Grade).HasColumnName("Grade").IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.GradeAssigned).HasColumnName("GradeAssigned");
        builder.Property(x => x.GradePublished).HasColumnName("GradePublished");
        builder.Property(x => x.GradeReleased).HasColumnName("GradeReleased");
        builder.Property(x => x.GradeWithheld).HasColumnName("GradeWithheld");
        builder.Property(x => x.GradeWithheldReason).HasColumnName("GradeWithheldReason").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.GradingProcess).HasColumnName("GradingProcess").IsUnicode(false).HasMaxLength(128);
        builder.Property(x => x.GradingStatus).HasColumnName("GradingStatus").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.LastChangeTime).HasColumnName("LastChangeTime");
        builder.Property(x => x.LastChangeType).HasColumnName("LastChangeType").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.LastChangeUser).HasColumnName("LastChangeUser").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.MaterialsIncludeDiagramBook).HasColumnName("MaterialsIncludeDiagramBook");
        builder.Property(x => x.MaterialsPackagedForDistribution).HasColumnName("MaterialsPackagedForDistribution").IsUnicode(false);
        builder.Property(x => x.MaterialsPermittedToCandidates).HasColumnName("MaterialsPermittedToCandidates").IsUnicode(false);
        builder.Property(x => x.RegistrationComment).HasColumnName("RegistrationComment").IsUnicode(false);
        builder.Property(x => x.RegistrationFee).HasColumnName("RegistrationFee").HasPrecision(11, 2);
        builder.Property(x => x.RegistrationIdentifier).HasColumnName("RegistrationIdentifier").IsRequired();
        builder.Property(x => x.RegistrationPassword).HasColumnName("RegistrationPassword").IsRequired().IsUnicode(false).HasMaxLength(14);
        builder.Property(x => x.RegistrationRequestedOn).HasColumnName("RegistrationRequestedOn");
        builder.Property(x => x.RegistrationSequence).HasColumnName("RegistrationSequence");
        builder.Property(x => x.RegistrationSource).HasColumnName("RegistrationSource").IsUnicode(false);
        builder.Property(x => x.SchoolIdentifier).HasColumnName("SchoolIdentifier");
        builder.Property(x => x.Score).HasColumnName("Score").HasPrecision(5, 4);
        builder.Property(x => x.SeatIdentifier).HasColumnName("SeatIdentifier");
        builder.Property(x => x.SynchronizationProcess).HasColumnName("SynchronizationProcess").IsUnicode(false);
        builder.Property(x => x.SynchronizationStatus).HasColumnName("SynchronizationStatus").IsUnicode(false).HasMaxLength(40);
        builder.Property(x => x.WorkBasedHoursToDate).HasColumnName("WorkBasedHoursToDate");
        builder.Property(x => x.IncludeInT2202).HasColumnName("IncludeInT2202").IsRequired();
        builder.Property(x => x.PaymentIdentifier).HasColumnName("PaymentIdentifier");
        builder.Property(x => x.CandidateType).HasColumnName("CandidateType").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.RegistrationRequestedBy).HasColumnName("RegistrationRequestedBy");
        builder.Property(x => x.AttendanceTaken).HasColumnName("AttendanceTaken");
        builder.Property(x => x.EligibilityUpdated).HasColumnName("EligibilityUpdated");
        builder.Property(x => x.BillingCode).HasColumnName("BillingCode").IsUnicode(false).HasMaxLength(100);

    }
}