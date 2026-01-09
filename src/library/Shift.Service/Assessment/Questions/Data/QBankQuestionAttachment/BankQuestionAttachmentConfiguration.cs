using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class BankQuestionAttachmentConfiguration : IEntityTypeConfiguration<BankQuestionAttachmentEntity>
{
    public void Configure(EntityTypeBuilder<BankQuestionAttachmentEntity> builder) 
    {
        builder.ToTable("QBankQuestionAttachment", "banks");
        builder.HasKey(x => new { x.QuestionIdentifier, x.UploadIdentifier });
            
        builder.Property(x => x.QuestionIdentifier).HasColumnName("QuestionIdentifier").IsRequired();
        builder.Property(x => x.UploadIdentifier).HasColumnName("UploadIdentifier").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier");

    }
}