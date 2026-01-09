using System.Data.Entity.ModelConfiguration;

using InSite.Application.Records.Read;

namespace InSite.Persistence
{
    public class TPrerequisiteConfiguration : EntityTypeConfiguration<TPrerequisite>
    {
        public TPrerequisiteConfiguration() : this("records") { }

        public TPrerequisiteConfiguration(string schema)
        {
            ToTable(schema + ".TPrerequisite");
            HasKey(x => new { x.PrerequisiteIdentifier });

            Property(x => x.ObjectIdentifier).IsRequired();
            Property(x => x.ObjectType).IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.OrganizationIdentifier).IsRequired();
            Property(x => x.PrerequisiteIdentifier).IsRequired();
            Property(x => x.TriggerChange).IsRequired().IsUnicode(false).HasMaxLength(30);
            Property(x => x.TriggerConditionScoreFrom).IsOptional();
            Property(x => x.TriggerConditionScoreThru).IsOptional();
            Property(x => x.TriggerIdentifier).IsRequired();
            Property(x => x.TriggerType).IsRequired().IsUnicode(false).HasMaxLength(30);
        }
    }
}
