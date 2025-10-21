using System.Data.Entity.ModelConfiguration;

using InSite.Application.Events.Read;

namespace InSite.Persistence
{
    public class QSeatConfiguration: EntityTypeConfiguration<QSeat>
    {
        public QSeatConfiguration() : this("events") { }

        public QSeatConfiguration(string schema)
        {
            ToTable(schema + ".QSeat");
            HasKey(x => new { x.SeatIdentifier });

            HasRequired(a => a.Event).WithMany(b => b.Seats).HasForeignKey(c => c.EventIdentifier);

            Property(x => x.Configuration).IsOptional().IsUnicode(false);
            Property(x => x.Content).IsOptional().IsUnicode(true);
            Property(x => x.SeatTitle).IsUnicode().IsUnicode(false).HasMaxLength(100);
        }
    }
}
