using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class FieldMapping : EntityTypeConfiguration<Field>
    {
        public FieldMapping() : this("custom_ncsha")
        {
        }

        public FieldMapping(string schema)
        {
            ToTable(schema + ".Field");
            HasKey(x => x.Code);

            Property(x => x.Category)
                .HasMaxLength(130)
                .IsRequired()
                .IsUnicode(false)
                ;

            Property(x => x.Code)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)
                .HasMaxLength(10)
                .IsRequired()
                .IsUnicode(false)
                ;

            Property(x => x.Name)
                .HasMaxLength(310)
                .IsRequired()
                .IsUnicode(false)
                ;

            Property(x => x.Unit)
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode(false)
                ;
        }
    }
}
