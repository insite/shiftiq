using System.Data.Entity.ModelConfiguration;

using InSite.Application.Events.Read;

namespace InSite.Persistence
{
    public class QEventAttendeeConfiguration : EntityTypeConfiguration<QEventAttendee>
    {
        public QEventAttendeeConfiguration() : this("events") { }

        public QEventAttendeeConfiguration(string schema)
        {
            ToTable(schema + ".QEventAttendee");
            HasKey(x => new { x.EventIdentifier, x.UserIdentifier });

            HasRequired(a => a.Event).WithMany(b => b.Attendees).HasForeignKey(c => c.EventIdentifier);
            HasRequired(a => a.Person).WithMany(b => b.EventAttendees).HasForeignKey(c => new {c.OrganizationIdentifier, c.UserIdentifier});

            Property(x => x.AttendeeRole).IsRequired().IsUnicode(false).HasMaxLength(200);
        }
    }
}
