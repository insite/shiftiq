using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.CMDS
{
    public class QInvoiceFeeConfiguration : EntityTypeConfiguration<QInvoiceFee>
    {
        public QInvoiceFeeConfiguration() : this("custom_cmds") { }

        public QInvoiceFeeConfiguration(string schema)
        {
            ToTable(schema + ".QCmdsInvoiceFee");
            HasKey(x => new { x.InvoiceFeeKey });

            Property(x => x.BillingClassification).IsRequired().IsUnicode(false).HasMaxLength(1);
            Property(x => x.CompanyName).IsOptional().IsUnicode(false).HasMaxLength(148);
            Property(x => x.FromDate).IsRequired();
            Property(x => x.InvoiceFeeKey).IsRequired();
            Property(x => x.PricePerUserPerPeriodPerCompany).IsRequired();
            Property(x => x.SharedCompanyCount).IsRequired();
            Property(x => x.ThruDate).IsRequired();
            Property(x => x.UserEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
        }
    }
}
