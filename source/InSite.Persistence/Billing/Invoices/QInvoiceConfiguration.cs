using System.Data.Entity.ModelConfiguration;

using InSite.Application.Invoices.Read;

namespace InSite.Persistence
{
    public class QInvoiceConfiguration : EntityTypeConfiguration<QInvoice>
    {
        public QInvoiceConfiguration() : this("invoices") { }

        public QInvoiceConfiguration(string schema)
        {
            ToTable(schema + ".QInvoice");
            HasKey(x => new { x.InvoiceIdentifier });
            Property(x => x.CustomerIdentifier).IsRequired();
            Property(x => x.InvoiceDrafted).IsOptional();
            Property(x => x.InvoiceIdentifier).IsRequired();
            Property(x => x.InvoicePaid).IsOptional();
            Property(x => x.InvoiceStatus).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.InvoiceSubmitted).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.EmployeeUserIdentifier).IsOptional();
            Property(x => x.BusinessCustomerGroupIdentifier).IsOptional();
            Property(x => x.IssueIdentifier).IsOptional();
        }
    }
}
