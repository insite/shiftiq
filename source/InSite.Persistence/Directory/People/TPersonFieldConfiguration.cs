using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TPersonFieldConfiguration : EntityTypeConfiguration<TPersonField>
    {
        public TPersonFieldConfiguration() : this("contacts") { }

        public TPersonFieldConfiguration(string schema)
        {
            ToTable(schema + ".TPersonField");
            HasKey(x => new { x.FieldIdentifier });

            Property(x => x.FieldIdentifier).IsRequired();
            Property(x => x.FieldName).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.FieldSequence).IsOptional();
            Property(x => x.FieldValue).IsRequired().IsUnicode(true);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UserIdentifier).IsRequired();

            HasRequired(a => a.User).WithMany(b => b.PersonFields).HasForeignKey(a => a.UserIdentifier).WillCascadeOnDelete(false);
        }
    }
}
