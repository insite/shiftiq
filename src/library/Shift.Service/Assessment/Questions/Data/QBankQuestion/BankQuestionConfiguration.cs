using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class BankQuestionConfiguration : IEntityTypeConfiguration<BankQuestionEntity>
{
    public void Configure(EntityTypeBuilder<BankQuestionEntity> builder) 
    {
        builder.ToTable("QBankQuestion", "banks");
        builder.HasKey(x => new { x.QuestionIdentifier });
            
        builder.Property(x => x.BankIdentifier).HasColumnName("BankIdentifier").IsRequired();
        builder.Property(x => x.BankIndex).HasColumnName("BankIndex").IsRequired();
        builder.Property(x => x.QuestionCode).HasColumnName("QuestionCode").IsUnicode(false);
        builder.Property(x => x.QuestionDifficulty).HasColumnName("QuestionDifficulty");
        builder.Property(x => x.QuestionIdentifier).HasColumnName("QuestionIdentifier").IsRequired();
        builder.Property(x => x.QuestionLikeItemGroup).HasColumnName("QuestionLikeItemGroup").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.QuestionReference).HasColumnName("QuestionReference").IsUnicode(false);
        builder.Property(x => x.QuestionTag).HasColumnName("QuestionTag").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.QuestionTaxonomy).HasColumnName("QuestionTaxonomy");
        builder.Property(x => x.QuestionText).HasColumnName("QuestionText").IsUnicode(false);
        builder.Property(x => x.CompetencyIdentifier).HasColumnName("CompetencyIdentifier");
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.QuestionCondition).HasColumnName("QuestionCondition").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.QuestionFlag).HasColumnName("QuestionFlag").IsUnicode(false).HasMaxLength(10);
        builder.Property(x => x.SetIdentifier).HasColumnName("SetIdentifier").IsRequired();
        builder.Property(x => x.QuestionFirstPublished).HasColumnName("QuestionFirstPublished");
        builder.Property(x => x.QuestionSourceIdentifier).HasColumnName("QuestionSourceIdentifier");
        builder.Property(x => x.QuestionSourceAssetNumber).HasColumnName("QuestionSourceAssetNumber").IsUnicode(false).HasMaxLength(16);
        builder.Property(x => x.QuestionAssetNumber).HasColumnName("QuestionAssetNumber").IsRequired().IsUnicode(false).HasMaxLength(16);
        builder.Property(x => x.LastChangeTime).HasColumnName("LastChangeTime");
        builder.Property(x => x.LastChangeType).HasColumnName("LastChangeType").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.LastChangeUser).HasColumnName("LastChangeUser").IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.ParentQuestionIdentifier).HasColumnName("ParentQuestionIdentifier");
        builder.Property(x => x.BankSubIndex).HasColumnName("BankSubIndex");
        builder.Property(x => x.QuestionType).HasColumnName("QuestionType").IsUnicode(false).HasMaxLength(21);
        builder.Property(x => x.QuestionTags).HasColumnName("QuestionTags").IsUnicode(false);
        builder.Property(x => x.RubricIdentifier).HasColumnName("RubricIdentifier");

    }
}