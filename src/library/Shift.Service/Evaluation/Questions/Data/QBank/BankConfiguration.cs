using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shift.Service.Assessment;

public class BankConfiguration : IEntityTypeConfiguration<BankEntity>
{
    public void Configure(EntityTypeBuilder<BankEntity> builder) 
    {
        builder.ToTable("QBank", "banks");
        builder.HasKey(x => new { x.BankIdentifier });
            
        builder.Property(x => x.AssetNumber).HasColumnName("AssetNumber").IsRequired();
        builder.Property(x => x.AttachmentCount).HasColumnName("AttachmentCount").IsRequired();
        builder.Property(x => x.BankIdentifier).HasColumnName("BankIdentifier").IsRequired();
        builder.Property(x => x.BankLevel).HasColumnName("BankLevel").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.BankName).HasColumnName("BankName").IsRequired().IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.BankSize).HasColumnName("BankSize").IsRequired();
        builder.Property(x => x.BankStatus).HasColumnName("BankStatus").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.BankTitle).HasColumnName("BankTitle").IsUnicode(false).HasMaxLength(200);
        builder.Property(x => x.BankType).HasColumnName("BankType").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.BankEdition).HasColumnName("BankEdition").IsUnicode(false).HasMaxLength(20);
        builder.Property(x => x.CommentCount).HasColumnName("CommentCount").IsRequired();
        builder.Property(x => x.FormCount).HasColumnName("FormCount").IsRequired();
        builder.Property(x => x.OptionCount).HasColumnName("OptionCount").IsRequired();
        builder.Property(x => x.QuestionCount).HasColumnName("QuestionCount").IsRequired();
        builder.Property(x => x.SetCount).HasColumnName("SetCount").IsRequired();
        builder.Property(x => x.SpecCount).HasColumnName("SpecCount").IsRequired();
        builder.Property(x => x.OrganizationIdentifier).HasColumnName("OrganizationIdentifier").IsRequired();
        builder.Property(x => x.FrameworkIdentifier).HasColumnName("FrameworkIdentifier");
        builder.Property(x => x.DepartmentIdentifier).HasColumnName("DepartmentIdentifier");
        builder.Property(x => x.IsActive).HasColumnName("IsActive").IsRequired();
        builder.Property(x => x.LastChangeTime).HasColumnName("LastChangeTime").IsRequired();
        builder.Property(x => x.LastChangeType).HasColumnName("LastChangeType").IsRequired().IsUnicode(false).HasMaxLength(100);
        builder.Property(x => x.LastChangeUser).HasColumnName("LastChangeUser").IsRequired().IsUnicode(false).HasMaxLength(100);

    }
}