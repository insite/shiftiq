using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
    public class TMeasurementConfiguration : EntityTypeConfiguration<TMeasurement>
    {
        public TMeasurementConfiguration() : this("reports") { }

        public TMeasurementConfiguration(string schema)
        {
            ToTable(schema + ".TMeasurement");
            HasKey(x => new { x.MeasurementIdentifier });

            Property(x => x.AsAt).IsRequired();
            Property(x => x.AsAtDate).IsRequired();
            Property(x => x.AsAtDay).IsRequired();
            Property(x => x.AsAtMonth).IsRequired();
            Property(x => x.AsAtQuarter).IsRequired();
            Property(x => x.AsAtWeek).IsRequired();
            Property(x => x.AsAtYear).IsRequired();
            Property(x => x.ContainerIdentifier).IsOptional();
            Property(x => x.ContainerType).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.IntervalType).IsRequired().IsUnicode(false).HasMaxLength(10);
            Property(x => x.MeasurementIdentifier).IsRequired();
            Property(x => x.QuantityDelta).IsOptional();
            Property(x => x.QuantityDeltaText).IsOptional().IsUnicode(false);
            Property(x => x.QuantityFunction).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.QuantityType).IsOptional().IsUnicode(false).HasMaxLength(10);
            Property(x => x.QuantityUnit).IsRequired().IsUnicode(false).HasMaxLength(20);
            Property(x => x.QuantityValue).IsRequired();
            Property(x => x.QuantityValueText).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.UniquePath).IsRequired().IsUnicode(false).HasMaxLength(704);
            Property(x => x.UniquePathHash).IsRequired();
            Property(x => x.VariableItem).IsRequired().IsUnicode(false).HasMaxLength(300);
            Property(x => x.VariableList).IsOptional().IsUnicode(false).HasMaxLength(100);
            Property(x => x.VariableRoot).IsOptional().IsUnicode(false).HasMaxLength(100);
        }
    }
}