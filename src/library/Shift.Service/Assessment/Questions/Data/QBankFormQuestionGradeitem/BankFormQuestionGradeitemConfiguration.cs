using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class BankFormQuestionGradeitemConfiguration : IEntityTypeConfiguration<BankFormQuestionGradeitemEntity>
{
    public void Configure(EntityTypeBuilder<BankFormQuestionGradeitemEntity> builder) 
    {
        builder.ToTable("QBankQuestionGradeItem", "banks");
        builder.HasKey(x => new { x.FormIdentifier, x.QuestionIdentifier });
            
        builder.Property(x => x.QuestionIdentifier).HasColumnName("QuestionIdentifier").IsRequired();
        builder.Property(x => x.FormIdentifier).HasColumnName("FormIdentifier").IsRequired();
        builder.Property(x => x.GradeItemIdentifier).HasColumnName("GradeItemIdentifier").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();

    }
}