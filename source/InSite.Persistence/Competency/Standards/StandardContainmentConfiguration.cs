using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class StandardContainmentConfiguration : EntityTypeConfiguration<StandardContainment>
    {
        public StandardContainmentConfiguration() : this("standards") { }

        public StandardContainmentConfiguration(string schema)
        {
            ToTable(schema + ".StandardContainment");
            HasKey(x => new { x.ParentStandardIdentifier,x.ChildStandardIdentifier });
            Property(x => x.ChildSequence).IsRequired();
            Property(x => x.ChildStandardIdentifier).IsRequired();
            Property(x => x.ParentStandardIdentifier).IsRequired();
            Property(x => x.CreditHours).IsOptional();
            Property(x => x.CreditType).IsOptional().IsUnicode(false).HasMaxLength(10);

            HasRequired(a => a.Child).WithMany(b => b.ChildContainments).HasForeignKey(a => a.ChildStandardIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.Parent).WithMany(b => b.ParentContainments).HasForeignKey(a => a.ParentStandardIdentifier).WillCascadeOnDelete(false);
        }
    }
}
