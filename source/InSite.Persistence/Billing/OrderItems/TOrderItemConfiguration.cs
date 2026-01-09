using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using InSite.Application.Invoices.Read;

namespace InSite.Persistence
{
    public class TOrderItemConfiguration : EntityTypeConfiguration<TOrderItem>
    {
        public TOrderItemConfiguration() : this("billing") { }

        public TOrderItemConfiguration(string schema)
        {
            ToTable("TOrderItem", schema);

            HasKey(x => x.OrderItemIdentifier);

            Property(x => x.ProductName).IsOptional().IsUnicode(false).HasMaxLength(254);
            Property(x => x.OrderItemType).IsOptional().IsUnicode(false).HasMaxLength(30);
            Property(x => x.UnitPrice).HasPrecision(12, 2);
            Property(x => x.LineTotalAmount).HasPrecision(12, 2).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed);

            HasRequired(x => x.Order).WithMany(x => x.OrderItems).HasForeignKey(x => x.OrderIdentifier);
            HasRequired(x => x.Product).WithMany(x => x.OrderItems).HasForeignKey(x => x.ProductIdentifier);
        }
    }
}
