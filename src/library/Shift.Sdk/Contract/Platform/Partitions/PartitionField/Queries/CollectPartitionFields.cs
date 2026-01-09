using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectPartitionFields : Query<IEnumerable<PartitionFieldModel>>, IPartitionFieldCriteria
    {
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}