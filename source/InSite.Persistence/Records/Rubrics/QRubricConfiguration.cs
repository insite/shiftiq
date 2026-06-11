using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class QRubricConfiguration : EntityTypeConfiguration<QRubric>
    {
        public QRubricConfiguration() : this("records") { }

        public QRubricConfiguration(string schema)
        {
            ToTable("QRubric", schema);
            HasKey(x => new { x.RubricIdentifier });

            Property(x => x.RubricTitle).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.RubricDescription).IsUnicode(false).HasMaxLength(800);
            Property(x => x.RubricPoints).HasPrecision(5, 2);
        }
    }
}
