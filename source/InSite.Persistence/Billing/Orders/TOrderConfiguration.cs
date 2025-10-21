using System.Data.Entity.ModelConfiguration;

using InSite.Application.Invoices.Read;

namespace InSite.Persistence
{
    public class TOrderConfiguration : EntityTypeConfiguration<TOrder>
    {
        public TOrderConfiguration() : this("billing") { }

        public TOrderConfiguration(string schema)
        {
            ToTable(schema + ".TOrder");
            HasKey(x => new { x.OrderIdentifier });
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.InvoiceItemIdentifier).IsRequired();
            Property(x => x.InvoiceIdentifier).IsRequired();
            Property(x => x.CustomerUserIdentifier).IsRequired();
            Property(x => x.ProductIdentifier).IsRequired();
            Property(x => x.ProductUrl).IsOptional().IsUnicode(false).HasMaxLength(500);
            Property(x => x.ProductName).IsOptional().IsUnicode(false).HasMaxLength(254);
            Property(x => x.ProductType).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.OrderCompleted).IsRequired();
            Property(x => x.DiscountAmount).HasPrecision(12, 2);
            Property(x => x.TaxAmount).HasPrecision(12, 2);
            Property(x => x.TaxRate).HasPrecision(5, 4);
            Property(x => x.TotalAmount).HasPrecision(12, 2);

            HasRequired(x => x.Invoice).WithMany(x => x.Orders).HasForeignKey(x => x.InvoiceIdentifier);
            HasRequired(x => x.Customer).WithMany(x => x.CustomerOrders).HasForeignKey(x => x.CustomerUserIdentifier);
            HasRequired(x => x.Manager).WithMany(x => x.ManagerOrders).HasForeignKey(x => x.CustomerUserIdentifier);
        }
    }
}
