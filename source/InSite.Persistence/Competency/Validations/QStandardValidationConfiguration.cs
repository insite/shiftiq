using System.Data.Entity.ModelConfiguration;

using InSite.Application.Standards.Read;

namespace InSite.Persistence
{
    internal class QStandardValidationConfiguration : EntityTypeConfiguration<QStandardValidation>
    {
        public QStandardValidationConfiguration() : this("standard") { }

        public QStandardValidationConfiguration(string schema)
        {
            ToTable("QStandardValidation", schema);
            HasKey(x => new { x.StandardValidationIdentifier });

            Property(x => x.SelfAssessmentStatus).IsUnicode(false).HasMaxLength(50);
            Property(x => x.ValidationComment).IsUnicode(false);
            Property(x => x.ValidationStatus).IsUnicode(false).HasMaxLength(50);
        }
    }
}
