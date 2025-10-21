using System;

namespace InSite.Persistence
{
	public partial class TPartitionSettingEntity
	{
        public Guid SettingIdentifier { get; set; }

        public string SettingName { get; set; }
        public string SettingValue { get; set; }
	}
}