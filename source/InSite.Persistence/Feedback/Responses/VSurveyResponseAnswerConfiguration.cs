using System.Data.Entity.ModelConfiguration;

using InSite.Application.Surveys.Read;

namespace InSite.Persistence
{
    public class VSurveyResponseAnswerConfiguration : EntityTypeConfiguration<VSurveyResponseAnswer>
    {
        public VSurveyResponseAnswerConfiguration() : this("surveys") { }

        public VSurveyResponseAnswerConfiguration(string schema)
        {
            ToTable(schema + ".VSurveyResponseAnswer");
            HasKey(x => new { x.RandomIdentifier });

            Property(x => x.AnswerText).IsOptional().IsUnicode(true);
            Property(x => x.OptionText).IsOptional().IsUnicode(true);
            Property(x => x.ResponseSessionIdentifier).IsRequired();
            Property(x => x.ResponseSessionStarted).IsOptional();
            Property(x => x.UserEmail).IsRequired().IsUnicode(false).HasMaxLength(254);
            Property(x => x.UserIdentifier).IsRequired();
            Property(x => x.UserName).IsRequired().IsUnicode(false).HasMaxLength(120);
        }
    }
}
