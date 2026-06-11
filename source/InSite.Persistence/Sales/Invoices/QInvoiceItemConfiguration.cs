using System.Data.Entity.ModelConfiguration;

using InSite.Application.Invoices.Read;

namespace InSite.Persistence
{
    public class QInvoiceItemConfiguration : EntityTypeConfiguration<QInvoiceItem>
    {
        public QInvoiceItemConfiguration() : this("invoices") { }

        public QInvoiceItemConfiguration(string schema)
        {
            ToTable(schema + ".QInvoiceItem");
            HasKey(x => x.ItemIdentifier);

            Property(x => x.InvoiceIdentifier).IsRequired();
            Property(x => x.ItemDescription).IsOptional().IsUnicode(false).HasMaxLength(400);
            Property(x => x.ItemIdentifier).IsRequired();
            Property(x => x.ItemPrice).IsRequired();
            Property(x => x.TaxRate).IsOptional();
            Property(x => x.ItemQuantity).IsRequired();
            Property(x => x.ItemSequence).IsRequired();
            Property(x => x.ProductIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();

            HasRequired(x => x.Product).WithMany(x => x.InvoiceItems).HasForeignKey(x => x.ProductIdentifier);
            HasRequired(x => x.Invoice).WithMany(x => x.InvoiceItems).HasForeignKey(x => x.InvoiceIdentifier);
            HasRequired(x => x.VInvoice).WithMany(x => x.InvoiceItems).HasForeignKey(x => x.InvoiceIdentifier);
        }
    }
}
