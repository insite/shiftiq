using System.Data.Entity.ModelConfiguration;

using InSite.Application.Sites.Read;

namespace InSite.Persistence
{
    public class VAssessmentPageConfiguration : EntityTypeConfiguration<VAssessmentPage>
    {
        public VAssessmentPageConfiguration() : this("sites") { }

        public VAssessmentPageConfiguration(string schema)
        {
            ToTable(schema + ".VAssessmentPage");
            HasKey(x => x.PageIdentifier);

            Property(x => x.FormAsset).IsRequired();
            Property(x => x.FormIdentifier).IsRequired();
            Property(x => x.FormName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.FormPublicationStatus).IsOptional().IsUnicode(false).HasMaxLength(50);
            Property(x => x.FormTitle).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.PageIsHidden).IsRequired();
            Property(x => x.PageIdentifier).IsRequired();
        }
    }
}
