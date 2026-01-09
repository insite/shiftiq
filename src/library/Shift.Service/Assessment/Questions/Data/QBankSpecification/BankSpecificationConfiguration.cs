using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class BankSpecificationConfiguration : IEntityTypeConfiguration<BankSpecificationEntity>
{
    public void Configure(EntityTypeBuilder<BankSpecificationEntity> builder) 
    {
        builder.ToTable("QBankSpecification", "banks");
        builder.HasKey(x => new { x.SpecIdentifier });
            
        builder.Property(x => x.BankIdentifier).HasColumnName("BankIdentifier").IsRequired();
        builder.Property(x => x.CalcDisclosure).HasColumnName("CalcDisclosure").IsRequired().IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.CalcPassingScore).HasColumnName("CalcPassingScore").IsRequired().HasPrecision(3, 2);
        builder.Property(x => x.SpecAsset).HasColumnName("SpecAsset").IsRequired();
        builder.Property(x => x.SpecConsequence).HasColumnName("SpecConsequence").IsRequired().IsUnicode(false).HasMaxLength(10);
        builder.Property(x => x.SpecFormCount).HasColumnName("SpecFormCount").IsRequired();
        builder.Property(x => x.SpecFormLimit).HasColumnName("SpecFormLimit").IsRequired();
        builder.Property(x => x.SpecIdentifier).HasColumnName("SpecIdentifier").IsRequired();
        builder.Property(x => x.SpecName).HasColumnName("SpecName").IsRequired().IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.SpecQuestionLimit).HasColumnName("SpecQuestionLimit").IsRequired();
        builder.Property(x => x.SpecType).HasColumnName("SpecType").IsRequired().IsUnicode(false).HasMaxLength(8);
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.CriterionTagCount).HasColumnName("CriterionTagCount");
        builder.Property(x => x.CriterionPivotCount).HasColumnName("CriterionPivotCount");
        builder.Property(x => x.CriterionAllCount).HasColumnName("CriterionAllCount");

    }
}