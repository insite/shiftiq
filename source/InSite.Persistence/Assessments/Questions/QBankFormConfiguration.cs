using System.Data.Entity.ModelConfiguration;

using InSite.Application.Banks.Read;

namespace InSite.Persistence
{
    public class QBankFormConfiguration : EntityTypeConfiguration<QBankForm>
    {
        public QBankFormConfiguration() : this("banks") { }

        public QBankFormConfiguration(string schema)
        {
            ToTable(schema + ".QBankForm");
            HasKey(x => new { x.FormIdentifier });

            Property(x => x.FormHook).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.FormName).IsRequired().IsUnicode(false).HasMaxLength(200);
            Property(x => x.FormOrigin).IsUnicode(false).HasMaxLength(100);
            Property(x => x.FormSource).IsUnicode(false).HasMaxLength(80);
            Property(x => x.FormPublicationStatus).IsUnicode(false).HasMaxLength(50);
            Property(x => x.FormTitle).IsUnicode(false).HasMaxLength(200);
            Property(x => x.FormType).IsUnicode(false).HasMaxLength(20);
            Property(x => x.FormClassificationInstrument).IsUnicode(false).HasMaxLength(100);
            Property(x => x.FormCode).IsUnicode(false).HasMaxLength(40);
            Property(x => x.BankLevelType).IsUnicode(false).HasMaxLength(20);
            Property(x => x.FormHasReferenceMaterials).IsUnicode(false).HasMaxLength(21);
            Property(x => x.FormPassingScore).IsOptional().HasPrecision(3, 2);
            Property(x => x.WhenAttemptCompletedNotifyAdminMessageIdentifier).IsOptional();
            Property(x => x.WhenAttemptStartedNotifyAdminMessageIdentifier).IsOptional();

            HasRequired(a => a.Bank).WithMany(b => b.Forms).HasForeignKey(c => c.BankIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.VBank).WithMany(b => b.Forms).HasForeignKey(c => c.BankIdentifier).WillCascadeOnDelete(false);
            HasRequired(a => a.BankSpecification).WithMany(b => b.Forms).HasForeignKey(c => c.SpecIdentifier).WillCascadeOnDelete(false);
        }
    }
}
