using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class PaProgramMapping : EntityTypeConfiguration<PaProgram>
    {
        public PaProgramMapping() : this("custom_ncsha")
        {
        }

        public PaProgramMapping(string schema)
        {
            ToTable(schema + ".PaProgram");
            HasKey(x => new { x.PaProgramId });

            Property(x => x.PaProgramId).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.AgencyGroupIdentifier);
            Property(x => x.RespondentUserIdentifier);
            Property(x => x.InsertedBy).IsUnicode(false).HasMaxLength(128);
            Property(x => x.PA001).IsUnicode(false).HasMaxLength(40);
            Property(x => x.PA002).IsUnicode(false).HasMaxLength(40);
            Property(x => x.PA003).IsUnicode(false).HasMaxLength(30);
            Property(x => x.PA004).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA005).IsUnicode(false).HasMaxLength(50);
            Property(x => x.PA006).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA007).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA008).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA009).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA010).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA011).IsUnicode(false).HasMaxLength(23);
            Property(x => x.PA012).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA013).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA014).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA015).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA016).IsUnicode(false).HasMaxLength(1010);
            Property(x => x.PA017).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA018).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA019).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA020).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA021).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA022).IsUnicode(false).HasMaxLength(1000);
            Property(x => x.PA023).IsUnicode(false).HasMaxLength(110);
            Property(x => x.PA024).IsUnicode(false).HasMaxLength(10);
            Property(x => x.PA025).IsUnicode(false).HasMaxLength(10);
            Property(x => x.PA026).IsUnicode(false).HasMaxLength(1240);
            Property(x => x.PA027).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA028).IsUnicode(false).HasMaxLength(860);
            Property(x => x.PA029).IsUnicode(false).HasMaxLength(1800);
            Property(x => x.PA030).IsUnicode(false).HasMaxLength(50);
            Property(x => x.PA031).IsUnicode(false).HasMaxLength(90);
            Property(x => x.PA032).IsUnicode(false).HasMaxLength(140);
            Property(x => x.PA033).IsUnicode(false).HasMaxLength(40);
            Property(x => x.PA034).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA035).IsUnicode(false).HasMaxLength(10);
            Property(x => x.PA036).IsUnicode(false).HasMaxLength(10);
            Property(x => x.PA037).IsUnicode(false).HasMaxLength(10);
            Property(x => x.PA038).IsUnicode(false).HasMaxLength(10);
            Property(x => x.PA039).IsUnicode(false).HasMaxLength(10);
            Property(x => x.PA040).IsUnicode(false).HasMaxLength(30);
            Property(x => x.PA041).IsUnicode(false).HasMaxLength(10);
            Property(x => x.PA042).IsUnicode(false).HasMaxLength(10);
            Property(x => x.PA043).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA044).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA045).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA046).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA047).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA048).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA049).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA050).IsUnicode(false).HasMaxLength(20);
            Property(x => x.PA051).IsUnicode(false).HasMaxLength(20);
            Property(x => x.RespondentName).IsUnicode(false).HasMaxLength(40);
            Property(x => x.StateName).IsUnicode(false).HasMaxLength(30);
            Property(x => x.UpdatedBy).IsUnicode(false).HasMaxLength(20);
            Property(x => x.IsVisibleOnTable01).IsRequired();
            Property(x => x.IsVisibleOnTable02).IsRequired();
            Property(x => x.IsVisibleOnTable03).IsRequired();
            Property(x => x.IsVisibleOnTable04).IsRequired();
            Property(x => x.ProgramFolderId);
            Property(x => x.SurveyYear).IsRequired();
            Property(x => x.DateTimeSaved);
            Property(x => x.DateTimeSubmitted);
            Property(x => x.Deadline);
            Property(x => x.InsertedOn);
            Property(x => x.UpdatedOn);
        }
    }
}