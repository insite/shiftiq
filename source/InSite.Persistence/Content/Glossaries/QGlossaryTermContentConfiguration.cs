using System.Data.Entity.ModelConfiguration;

using InSite.Application.Glossaries.Read;

namespace InSite.Persistence
{
    public class QGlossaryTermContentConfiguration : EntityTypeConfiguration<QGlossaryTermContent>
    {
        public QGlossaryTermContentConfiguration() : this("assets") { }

        public QGlossaryTermContentConfiguration(string schema)
        {
            ToTable(schema + ".QGlossaryTermContent");
            HasKey(x => x.RelationshipIdentifier);

            Property(x => x.ContainerType).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.ContentLabel).IsRequired().IsUnicode(false).HasMaxLength(100);

            HasRequired(a => a.Term).WithMany(b => b.TermContents).HasForeignKey(c => c.TermIdentifier).WillCascadeOnDelete(false);
        }
    }
}