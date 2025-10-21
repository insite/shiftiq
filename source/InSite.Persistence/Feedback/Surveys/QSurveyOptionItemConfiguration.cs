using System.Data.Entity.ModelConfiguration;

using InSite.Application.Surveys.Read;

namespace InSite.Persistence
{
    public class QSurveyOptionItemConfiguration : EntityTypeConfiguration<QSurveyOptionItem>
    {
        public QSurveyOptionItemConfiguration() : this("surveys") { }

        public QSurveyOptionItemConfiguration(string schema)
        {
            ToTable(schema + ".QSurveyOptionItem");
            HasKey(x => new { x.SurveyOptionItemIdentifier });
            Property(x => x.BranchToQuestionIdentifier).IsOptional();
            Property(x => x.SurveyOptionItemCategory).IsOptional().IsUnicode(false).HasMaxLength(120);
            Property(x => x.SurveyOptionItemIdentifier).IsRequired();
            Property(x => x.SurveyOptionItemPoints).IsOptional();
            Property(x => x.SurveyOptionItemSequence).IsRequired();
            Property(x => x.SurveyOptionListIdentifier).IsRequired();

            HasOptional(a => a.BranchToQuestion).WithMany(b => b.BranchFromOptions).HasForeignKey(c => c.BranchToQuestionIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.SurveyOptionList).WithMany(b => b.QSurveyOptionItems).HasForeignKey(c => c.SurveyOptionListIdentifier).WillCascadeOnDelete(false);
        }
    }
}
