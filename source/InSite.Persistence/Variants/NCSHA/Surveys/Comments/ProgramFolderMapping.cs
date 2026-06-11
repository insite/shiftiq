using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class ProgramFolderMapping : EntityTypeConfiguration<ProgramFolder>
    {
        public ProgramFolderMapping() : this("custom_ncsha")
        {
        }

        public ProgramFolderMapping(string schema)
        {
            ToTable(schema + ".ProgramFolder");
            HasKey(x => new { x.ProgramFolderId });

            Property(x => x.ProgramFolderId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                ;
        }
    }
}