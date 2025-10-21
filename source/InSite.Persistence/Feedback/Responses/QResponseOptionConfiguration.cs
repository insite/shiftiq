using System.Data.Entity.ModelConfiguration;

using InSite.Application.Surveys.Read;

namespace InSite.Persistence
{
    public class QResponseOptionConfiguration : EntityTypeConfiguration<QResponseOption>
    {
        public QResponseOptionConfiguration() : this("surveys") { }

        public QResponseOptionConfiguration(string schema)
        {
            ToTable(schema + ".QResponseOption");
            HasKey(x => new { x.ResponseSessionIdentifier,x.SurveyOptionIdentifier });
            Property(x => x.ResponseOptionIsSelected).IsRequired();
            Property(x => x.ResponseSessionIdentifier).IsRequired();
            Property(x => x.SurveyOptionIdentifier).IsRequired();

            HasRequired(a => a.ResponseSession).WithMany(b => b.QResponseOptions).HasForeignKey(c => c.ResponseSessionIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.SurveyOptionItem).WithMany(b => b.QResponseOptions).HasForeignKey(c => c.SurveyOptionIdentifier).WillCascadeOnDelete(false);
        }
    }
}
