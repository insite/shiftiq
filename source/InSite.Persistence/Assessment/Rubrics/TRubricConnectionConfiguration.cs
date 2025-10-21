using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    internal class TRubricConnectionConfiguration : EntityTypeConfiguration<TRubricConnection>
    {
        public TRubricConnectionConfiguration() : this("records") { }

        public TRubricConnectionConfiguration(string schema)
        {
            ToTable(schema + ".TRubricConnection");
            HasKey(x => x.ConnectionIdentifier);

            Property(x => x.RubricIdentifier).IsRequired();
            Property(x => x.QuestionIdentifier).IsRequired();
            Property(x => x.ConnectionIdentifier).IsRequired();

            HasRequired(a => a.Rubric).WithMany(b => b.Connections).HasForeignKey(c => c.RubricIdentifier);
        }
    }
}
