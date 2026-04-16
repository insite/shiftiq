using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class BankQuestionConfiguration : IEntityTypeConfiguration<BankQuestionEntity>
{
    public void Configure(EntityTypeBuilder<BankQuestionEntity> builder) 
    {
        builder.ToTable("QBankQuestion", "banks");
        builder.HasKey(x => new { x.QuestionIdentifier });

        builder.Property(x => x.BankIdentifier).IsRequired();
        builder.Property(x => x.BankIndex).IsRequired();
        builder.Property(x => x.CompetencyIdentifier);
        builder.Property(x => x.QuestionCode).IsUnicode(false);
        builder.Property(x => x.QuestionDifficulty);
        builder.Property(x => x.QuestionFlag).IsUnicode(false).HasMaxLength(10);
        builder.Property(x => x.QuestionIdentifier).IsRequired();
        builder.Property(x => x.QuestionLikeItemGroup).IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.QuestionReference).IsUnicode(false);
        builder.Property(x => x.QuestionCondition).IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.QuestionTag).IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.QuestionTags).IsUnicode(false);
        builder.Property(x => x.QuestionTaxonomy);
        builder.Property(x => x.QuestionText).IsUnicode(false);
        builder.Property(x => x.SetIdentifier).IsRequired();
        builder.Property(x => x.OrganizationIdentifier).IsRequired();
        builder.Property(x => x.QuestionAssetNumber).IsRequired().IsUnicode(false).HasMaxLength(16);
        builder.Property(x => x.QuestionSourceAssetNumber).IsUnicode(false).HasMaxLength(16);
        builder.Property(x => x.LastChangeType).IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.LastChangeUser).IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.QuestionType).IsUnicode(false).HasMaxLength(21);
        builder.Property(x => x.QuestionPublicationStatus).IsUnicode(false).HasMaxLength(12);
        builder.Property(x => x.QuestionPoints).HasPrecision(7, 2);
        builder.Property(x => x.QuestionCutScore).HasPrecision(7, 2);
        builder.Property(x => x.QuestionCalculationMethod).IsUnicode(false).HasMaxLength(23);
        builder.Property(x => x.SetName).IsUnicode(false).HasMaxLength(64);
    }
}