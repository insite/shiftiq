using System.Data.Entity.ModelConfiguration;

using InSite.Application.Payments.Read;

namespace InSite.Persistence
{
    public class QPaymentConfiguration : EntityTypeConfiguration<QPayment>
    {
        public QPaymentConfiguration() : this("payments") { }

        public QPaymentConfiguration(string schema)
        {
            ToTable(schema + ".QPayment");
            HasKey(x => new { x.PaymentIdentifier });

            Property(x => x.CustomerIP).IsOptional().IsUnicode(false).HasMaxLength(15);
            Property(x => x.PaymentAborted).IsOptional();
            Property(x => x.PaymentAmount).IsRequired();
            Property(x => x.PaymentApproved).IsOptional();
            Property(x => x.PaymentDeclined).IsOptional();
            Property(x => x.PaymentIdentifier).IsRequired();
            Property(x => x.PaymentStarted).IsOptional();
            Property(x => x.PaymentStatus).IsRequired().IsUnicode(false).HasMaxLength(10);
            Property(x => x.ResultCode).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.ResultMessage).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.OrganizationIdentifier).IsRequired();

            HasRequired(a => a.CreatedByUser).WithMany(b => b.Payments).HasForeignKey(c => c.CreatedBy).WillCascadeOnDelete(false);
            HasRequired(a => a.CreatedInvoice).WithMany(b => b.Payments).HasForeignKey(c => c.InvoiceIdentifier).WillCascadeOnDelete(false);
        }
    }
}
