using System.Data.Entity.ModelConfiguration;

using InSite.Application.Glossaries.Read;

namespace InSite.Persistence
{
    public class QGlossaryTermConfiguration : EntityTypeConfiguration<QGlossaryTerm>
    {
        public QGlossaryTermConfiguration() : this("assets") { }

        public QGlossaryTermConfiguration(string schema)
        {
            ToTable(schema + ".QGlossaryTerm");
            HasKey(x => new { x.TermIdentifier });

            Property(x => x.Approved).IsOptional();
            Property(x => x.ApprovedBy).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.LastRevised).IsOptional();
            Property(x => x.LastRevisedBy).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.Proposed).IsRequired();
            Property(x => x.ProposedBy).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.RevisionCount).IsRequired();
            Property(x => x.TermName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.TermStatus).IsRequired().IsUnicode(false).HasMaxLength(10);
        }
    }
}