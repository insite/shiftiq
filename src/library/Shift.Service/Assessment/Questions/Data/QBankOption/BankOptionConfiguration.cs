using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class BankOptionConfiguration : IEntityTypeConfiguration<BankOptionEntity>
{
    public void Configure(EntityTypeBuilder<BankOptionEntity> builder) 
    {
        builder.ToTable("QBankOption", "banks");
        builder.HasKey(x => new { x.OptionKey, x.QuestionIdentifier });
            
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.BankIdentifier).HasColumnName("BankIdentifier").IsRequired();
        builder.Property(x => x.SetIdentifier).HasColumnName("SetIdentifier").IsRequired();
        builder.Property(x => x.QuestionIdentifier).HasColumnName("QuestionIdentifier").IsRequired();
        builder.Property(x => x.OptionKey).HasColumnName("OptionKey").IsRequired();
        builder.Property(x => x.CompetencyIdentifier).HasColumnName("CompetencyIdentifier");
        builder.Property(x => x.OptionText).HasColumnName("OptionText").IsUnicode(false);

    }
}