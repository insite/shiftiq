using System.Data.Entity.ModelConfiguration;

using InSite.Application.Registrations.Read;

namespace InSite.Persistence
{
    public class QRegistrationConfiguration : EntityTypeConfiguration<QRegistration>
    {
        public QRegistrationConfiguration() : this("registrations") { }

        public QRegistrationConfiguration(string schema)
        {
            ToTable(schema + ".QRegistration");
            HasKey(x => new { x.RegistrationIdentifier });

            HasRequired(a => a.Event).WithMany(b => b.Registrations).HasForeignKey(c => c.EventIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Candidate).WithMany(b => b.Registrations).HasForeignKey(c => new { c.OrganizationIdentifier, c.CandidateIdentifier }).WillCascadeOnDelete(false);
            HasOptional(a => a.Form).WithMany(b => b.Registrations).HasForeignKey(c => c.ExamFormIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Seat).WithMany(b => b.Registrations).HasForeignKey(c => c.SeatIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Attempt).WithMany(b => b.Registrations).HasForeignKey(c => c.AttemptIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Employer).WithMany(b => b.EmployerRegistrations).HasForeignKey(c => c.EmployerIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Customer).WithMany(b => b.CustomerRegistrations).HasForeignKey(c => c.CustomerIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.Payment).WithMany(b => b.Registrations).HasForeignKey(c => c.PaymentIdentifier).WillCascadeOnDelete(false);
            HasOptional(a => a.RegistrationRequestedByPerson).WithMany(b => b.RequestedRegistrations).HasForeignKey(c => new { c.OrganizationIdentifier, c.RegistrationRequestedBy }).WillCascadeOnDelete(false);

            Property(x => x.ApprovalProcess).IsOptional().IsUnicode(false);
            Property(x => x.ApprovalReason).IsOptional().IsUnicode(false);
            Property(x => x.ApprovalStatus).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.AttemptIdentifier).IsOptional();
            Property(x => x.AttendanceStatus).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.BillingCustomer).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.CandidateIdentifier).IsRequired();
            Property(x => x.CandidateType).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.CustomerIdentifier).IsOptional();
            Property(x => x.DistributionExpected).IsOptional();
            Property(x => x.EligibilityProcess).IsOptional().IsUnicode(false);
            Property(x => x.EligibilityStatus).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.EmployerIdentifier).IsOptional();
            Property(x => x.EventIdentifier).IsRequired();
            Property(x => x.EventPotentialConflicts).IsOptional().IsUnicode(false);
            Property(x => x.ExamFormIdentifier).IsOptional();
            Property(x => x.ExamTimeLimit).IsOptional();
            Property(x => x.Grade).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.GradeAssigned).IsOptional();
            Property(x => x.GradePublished).IsOptional();
            Property(x => x.GradeReleased).IsOptional();
            Property(x => x.GradeWithheld).IsOptional();
            Property(x => x.GradeWithheldReason).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.GradingProcess).IsOptional().IsUnicode(false).HasMaxLength(128);
            Property(x => x.GradingStatus).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.IncludeInT2202).IsRequired();
            Property(x => x.LastChangeTime).IsOptional();
            Property(x => x.LastChangeType).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LastChangeUser).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.MaterialsIncludeDiagramBook).IsOptional();
            Property(x => x.MaterialsPackagedForDistribution).IsOptional().IsUnicode(false);
            Property(x => x.MaterialsPermittedToCandidates).IsOptional().IsUnicode(false);
            Property(x => x.PaymentIdentifier).IsOptional();
            Property(x => x.RegistrationComment).IsOptional().IsUnicode(false);
            Property(x => x.RegistrationFee).IsOptional();
            Property(x => x.RegistrationIdentifier).IsRequired();
            Property(x => x.RegistrationPassword).IsRequired().IsUnicode(false).HasMaxLength(14);
            Property(x => x.RegistrationRequestedOn).IsOptional();
            Property(x => x.RegistrationSequence).IsOptional();
            Property(x => x.RegistrationSource).IsOptional().IsUnicode(false);
            Property(x => x.SchoolIdentifier).IsOptional();
            Property(x => x.Score).HasPrecision(5, 4).IsOptional();
            Property(x => x.SeatIdentifier).IsOptional();
            Property(x => x.SynchronizationProcess).IsOptional().IsUnicode(false);
            Property(x => x.SynchronizationStatus).IsOptional().IsUnicode(false).HasMaxLength(40);
            Property(x => x.WorkBasedHoursToDate).IsOptional();
            Property(x => x.BillingCode).IsUnicode(false).HasMaxLength(100);
        }
    }
}
