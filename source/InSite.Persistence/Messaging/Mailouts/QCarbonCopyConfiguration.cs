using InSite.Application.Messages.Read;

namespace InSite.Persistence
{
    public class QCarbonCopyConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<QCarbonCopy>
    {
        public QCarbonCopyConfiguration()
        {
            ToTable("QCarbonCopy", "communications");
            HasKey(x => new { x.CarbonCopyIdentifier });

            Property(x => x.CarbonCopyIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.RecipientIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
        }
    }
}