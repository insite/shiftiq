using System.Data.Entity.ModelConfiguration;

using InSite.Application.Banks.Read;

namespace InSite.Persistence
{
    public class VBankConfiguration : EntityTypeConfiguration<VBank>
    {
        public VBankConfiguration() : this("banks") { }

        public VBankConfiguration(string schema)
        {
            ToTable(schema + ".VBank");
            HasKey(x => new { x.BankIdentifier });

            Property(x => x.AssetNumber).IsRequired();
            Property(x => x.AttachmentCount).IsRequired();
            Property(x => x.BankEdition).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.BankIdentifier).IsRequired();
            Property(x => x.BankLevel).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.BankName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.BankSize).IsRequired();
            Property(x => x.BankStatus).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.BankTitle).IsOptional().IsUnicode(false).HasMaxLength(200);
            Property(x => x.BankType).IsOptional().IsUnicode(false).HasMaxLength(20);
            Property(x => x.CommentCount).IsRequired();
            Property(x => x.FormCount).IsRequired();
            Property(x => x.FrameworkIdentifier).IsOptional();
            Property(x => x.FrameworkTitle).IsOptional().IsUnicode(true).HasMaxLength(100);
            Property(x => x.OccupationIdentifier).IsOptional();
            Property(x => x.OccupationTitle).IsOptional().IsUnicode(true).HasMaxLength(100);
            Property(x => x.OptionCount).IsRequired();
            Property(x => x.QuestionCount).IsRequired();
            Property(x => x.SetCount).IsRequired();
            Property(x => x.SpecCount).IsRequired();
            Property(x => x.OrganizationIdentifier).IsRequired();
        }
    }
}
