using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class BankSpecificationConfiguration : IEntityTypeConfiguration<BankSpecificationEntity>
{
    public void Configure(EntityTypeBuilder<BankSpecificationEntity> builder) 
    {
        builder.ToTable("QBankSpecification", "banks");
        builder.HasKey(x => x.SpecIdentifier);

        builder.Property(x => x.BankIdentifier).IsRequired();
        builder.Property(x => x.CalcDisclosure).IsRequired().IsUnicode(false).HasMaxLength(30);
        builder.Property(x => x.CalcPassingScore).IsRequired().HasPrecision(3, 2);
        builder.Property(x => x.CriterionAllCount);
        builder.Property(x => x.CriterionTagCount);
        builder.Property(x => x.CriterionPivotCount);
        builder.Property(x => x.SpecAsset).IsRequired();
        builder.Property(x => x.SpecConsequence).IsRequired().IsUnicode(false).HasMaxLength(10);
        builder.Property(x => x.SpecFormCount).IsRequired();
        builder.Property(x => x.SpecFormLimit).IsRequired();
        builder.Property(x => x.SpecIdentifier).IsRequired();
        builder.Property(x => x.SpecName).IsRequired().IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.SpecQuestionLimit).IsRequired();
        builder.Property(x => x.SpecType).IsRequired().IsUnicode(false).HasMaxLength(8);
        builder.Property(x => x.OrganizationIdentifier).IsRequired();
    }
}
