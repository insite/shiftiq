using System.Data.Entity.ModelConfiguration;

using InSite.Application.Surveys.Read;

namespace InSite.Persistence
{
    public class QSurveyOptionListConfiguration : EntityTypeConfiguration<QSurveyOptionList>
    {
        public QSurveyOptionListConfiguration() : this("surveys") { }

        public QSurveyOptionListConfiguration(string schema)
        {
            ToTable(schema + ".QSurveyOptionList");
            HasKey(x => new { x.SurveyOptionListIdentifier });
            Property(x => x.SurveyOptionListIdentifier).IsRequired();
            Property(x => x.SurveyOptionListSequence).IsRequired();

            HasRequired(a => a.SurveyQuestion).WithMany(b => b.QSurveyOptionLists).HasForeignKey(c => c.SurveyQuestionIdentifier).WillCascadeOnDelete(false);
        }
    }
}
