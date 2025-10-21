using System.Data.Entity.ModelConfiguration;

using InSite.Application.Surveys.Read;

namespace InSite.Persistence
{
    public class VResponseFirstSelectionConfiguration : EntityTypeConfiguration<VResponseFirstSelection>
    {
        public VResponseFirstSelectionConfiguration()
        {
            ToTable("VResponseFirstSelection", "surveys");
            HasKey(x => x.ResponseIdentifier);

            Property(x => x.ResponseIdentifier).IsRequired();
            Property(x => x.SurveyIdentifier).IsRequired();
            Property(x => x.AnswerText).IsUnicode(true).HasMaxLength(2147483647);
            Property(x => x.QuestionType).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.QuestionSequence).IsRequired();
        }
    }
}
