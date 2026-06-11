using System.Data.Entity.ModelConfiguration;

using InSite.Application.Invoices.Read;

namespace InSite.Persistence
{
    public class VInvoiceConfiguration : EntityTypeConfiguration<VInvoice>
    {
        public VInvoiceConfiguration() : this("invoices") { }

        public VInvoiceConfiguration(string schema)
        {
            ToTable(schema + ".VInvoice");
            HasKey(x => new { x.InvoiceIdentifier });

            Property(x => x.CustomerEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.CustomerFullName).IsRequired().IsUnicode(false).HasMaxLength(120);
            Property(x => x.CustomerIdentifier).IsRequired();
            Property(x => x.BusinessCustomerGroupIdentifier).IsOptional();
            Property(x => x.EmployeeUserIdentifier).IsOptional();
            Property(x => x.IssueIdentifier).IsOptional();
            Property(x => x.BusinessCustomerName).IsOptional();
            Property(x => x.EmployeeName).IsOptional();
            Property(x => x.IssueTitle).IsOptional();
            Property(x => x.InvoiceAmount).IsOptional();
            Property(x => x.InvoiceDrafted).IsOptional();
            Property(x => x.InvoiceIdentifier).IsRequired();
            Property(x => x.InvoicePaid).IsOptional();
            Property(x => x.InvoiceStatus).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.InvoiceSubmitted).IsOptional();
            Property(x => x.ItemCount).IsOptional();
            Property(x => x.OrganizationIdentifier).IsRequired();
        }
    }
}
