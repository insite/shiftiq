using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class CounterMapping : EntityTypeConfiguration<Counter>
    {
        public CounterMapping() : this("custom_ncsha")
        {
        }

        public CounterMapping(string schema)
        {
            ToTable(schema + ".Counter");
            HasKey(x => x.CounterIdentifier);

            Property(x => x.CounterIdentifier).IsRequired();

            Property(x => x.Category)
                .HasMaxLength(120)
                .IsRequired()
                .IsUnicode(false)
                ;

            Property(x => x.Code)
                .HasMaxLength(10)
                .IsRequired()
                .IsUnicode(false)
                ;

            Property(x => x.Name)
                .HasMaxLength(310)
                .IsRequired()
                .IsUnicode(false)
                ;

            Property(x => x.Scope)
                .HasMaxLength(40)
                .IsRequired()
                .IsUnicode(false)
                ;

            Property(x => x.Value)
                .HasPrecision(16,2)
                ;

            Property(x => x.Unit)
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode(false)
                ;
        }
    }
}