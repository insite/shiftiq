using System.Data.Entity.ModelConfiguration;

using InSite.Application.Invoices.Read;

namespace InSite.Persistence
{
    public class VEventRegistrationPaymentConfiguration : EntityTypeConfiguration<VEventRegistrationPayment>
    {
        public VEventRegistrationPaymentConfiguration() : this("invoices") { }

        public VEventRegistrationPaymentConfiguration(string schema)
        {
            ToTable(schema + ".VEventRegistrationPayment");
            HasKey(x => x.RegistrationIdentifier);

            Property(x => x.EmployerName).IsOptional().IsUnicode(false).HasMaxLength(148);
            Property(x => x.EventDate).IsRequired();
            Property(x => x.EventName).IsOptional().IsUnicode(false).HasMaxLength(400);
            Property(x => x.InvoiceIdentifier).IsRequired();
            Property(x => x.InvoiceNumber).IsOptional();
            Property(x => x.InvoiceSubmitted).IsOptional();
            Property(x => x.LearnerAttendee).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.OrderNumber).IsOptional().IsUnicode(false);
            Property(x => x.OrganizationCode).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.OrganizationName).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.PaymentIdentifier).IsRequired();
            Property(x => x.RegistrantCardholder).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.TransactionAmount).IsRequired();
            Property(x => x.TransactionCode).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.TransactionDate).IsOptional();
            Property(x => x.InvoiceStatus).IsOptional();
            Property(x => x.AchievementTitle).IsOptional();
            Property(x => x.TransactionStatus).IsRequired().IsUnicode(false).HasMaxLength(10);
            Property(x => x.CurrentAttendanceStatus).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.CurrentRegistrationStatus).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.SeatTitle).IsUnicode().IsUnicode(false);
            Property(x => x.AttendanceTaken).IsOptional();
        }
    }
}
