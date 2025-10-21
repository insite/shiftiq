using System.Data.Entity.ModelConfiguration;

using InSite.Application.Surveys.Read;

namespace InSite.Persistence
{
    public class QSurveyConditionConfiguration : EntityTypeConfiguration<QSurveyCondition>
    {
        public QSurveyConditionConfiguration() : this("surveys") { }

        public QSurveyConditionConfiguration(string schema)
        {
            ToTable(schema + ".QSurveyCondition");
            HasKey(x => new { x.MaskingSurveyOptionItemIdentifier,x.MaskedSurveyQuestionIdentifier });
            Property(x => x.MaskedSurveyQuestionIdentifier).IsRequired();
            Property(x => x.MaskingSurveyOptionItemIdentifier).IsRequired();

            HasRequired(a => a.MaskingSurveyOptionItem).WithMany(b => b.QSurveyConditions).HasForeignKey(c => c.MaskingSurveyOptionItemIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.MaskedSurveyQuestion).WithMany(b => b.QSurveyConditions).HasForeignKey(c => c.MaskedSurveyQuestionIdentifier).WillCascadeOnDelete(false);
        }
    }
}
