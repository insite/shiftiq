using System.Data.Entity.ModelConfiguration;

using InSite.Application.Banks.Read;

namespace InSite.Persistence
{
    public class QBankSpecificationConfiguration : EntityTypeConfiguration<QBankSpecification>
    {
        public QBankSpecificationConfiguration() : this("banks") { }

        public QBankSpecificationConfiguration(string schema)
        {
            ToTable(schema + ".QBankSpecification");
            HasKey(x => new { x.SpecIdentifier });

            Property(x => x.BankIdentifier).IsRequired();
            Property(x => x.CalcDisclosure).IsRequired().IsUnicode(false).HasMaxLength(30);
            Property(x => x.CalcPassingScore).IsRequired().HasPrecision(3, 2);
            Property(x => x.CriterionAllCount).IsOptional();
            Property(x => x.CriterionTagCount).IsOptional();
            Property(x => x.CriterionPivotCount).IsOptional();
            Property(x => x.SpecAsset).IsRequired();
            Property(x => x.SpecConsequence).IsRequired().IsUnicode(false).HasMaxLength(10);
            Property(x => x.SpecFormCount).IsRequired();
            Property(x => x.SpecFormLimit).IsRequired();
            Property(x => x.SpecIdentifier).IsRequired();
            Property(x => x.SpecName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.SpecQuestionLimit).IsRequired();
            Property(x => x.SpecType).IsRequired().IsUnicode(false).HasMaxLength(8);
            Property(x => x.OrganizationIdentifier).IsRequired();

            HasRequired(x => x.Bank).WithMany(x => x.Specifications).HasForeignKey(x => x.BankIdentifier);
        }
    }
}
