using System.Data.Entity.ModelConfiguration;

using InSite.Application.Contacts.Read;

namespace InSite.Persistence
{
    public class VPersonConfiguration : EntityTypeConfiguration<VPerson>
    {
        public VPersonConfiguration() : this("contacts") { }

        public VPersonConfiguration(string schema)
        {
            ToTable(schema + ".VPerson");
            HasKey(x => new { x.OrganizationIdentifier, x.UserIdentifier });

            HasRequired(a => a.User).WithMany(b => b.Persons).HasForeignKey(c => c.UserIdentifier);

            Property(x => x.IsLearner).IsRequired();
            Property(x => x.PersonCode).IsUnicode(false).HasMaxLength(20);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();

            HasOptional(a => a.BillingAddress).WithMany(b => b.BillingPersons).HasForeignKey(c => c.BillingAddressIdentifier);
            HasOptional(a => a.HomeAddress).WithMany(b => b.HomePersons).HasForeignKey(c => c.HomeAddressIdentifier);
            HasOptional(a => a.ShippingAddress).WithMany(b => b.ShippingPersons).HasForeignKey(c => c.ShippingAddressIdentifier);
            HasOptional(a => a.WorkAddress).WithMany(b => b.WorkPersons).HasForeignKey(c => c.WorkAddressIdentifier);
        }
    }
}