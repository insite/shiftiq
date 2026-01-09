using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class FilterMapping : EntityTypeConfiguration<Filter>
    {
        public FilterMapping() : this("custom_ncsha")
        {
        }

        public FilterMapping(string schema)
        {
            ToTable(schema + ".Filter");
            HasKey(x => x.FilterId);

            Property(x => x.FilterId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                ;
            Property(x => x.FilterName)
                .HasMaxLength(128)
                .IsRequired()
                .IsUnicode(false)
                ;
            Property(x => x.FilterData)
                .IsMaxLength()
                .IsUnicode(false)
                .IsRequired()
                ;
        }
    }
}
