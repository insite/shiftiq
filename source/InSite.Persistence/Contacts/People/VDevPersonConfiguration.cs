using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    internal class VDevPersonConfiguration : EntityTypeConfiguration<VDevPerson>
    {
        public VDevPersonConfiguration()
        {
            ToTable("VDevPerson", "contacts");
            HasKey(x => new { x.UserIdentifier, x.OrganizationIdentifier });
        }
    }
}
