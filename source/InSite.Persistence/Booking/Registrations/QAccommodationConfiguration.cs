using System.Data.Entity.ModelConfiguration;

using InSite.Application.Registrations.Read;

namespace InSite.Persistence
{
    public class QAccommodationConfiguration : EntityTypeConfiguration<QAccommodation>
    {
        public QAccommodationConfiguration() : this("registrations") { }

        public QAccommodationConfiguration(string schema)
        {
            ToTable(schema + ".QAccommodation");
            HasKey(x => new { x.AccommodationIdentifier });

            Property(x => x.AccommodationIdentifier).IsRequired();
            Property(x => x.RegistrationIdentifier).IsRequired();
            Property(x => x.AccommodationName).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.AccommodationType).IsOptional().IsUnicode(false).HasMaxLength(50);

            HasRequired(a => a.Registration).WithMany(b => b.Accommodations).HasForeignKey(c => c.RegistrationIdentifier).WillCascadeOnDelete(false);
        }
    }
}
