using System.Data.Entity.ModelConfiguration;

namespace InSite.Persistence
{
	public class TPartitionSettingConfiguration : EntityTypeConfiguration<TPartitionSettingEntity>
	{
	    public TPartitionSettingConfiguration() : this("security") { }
	
		public TPartitionSettingConfiguration(string schema)
		{
			ToTable(schema + ".TPartitionSetting");
			
			HasKey(x => new { x.SettingIdentifier });

            Property(x => x.SettingIdentifier).HasColumnName("SettingIdentifier").IsRequired();
            Property(x => x.SettingName).HasColumnName("SettingName").IsRequired().IsUnicode(false).HasMaxLength(100);
            Property(x => x.SettingValue).HasColumnName("SettingValue").IsRequired().IsUnicode(false).HasMaxLength(500);

		}
	}
}