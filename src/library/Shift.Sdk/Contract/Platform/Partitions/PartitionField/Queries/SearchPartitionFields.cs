using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchPartitionFields : Query<IEnumerable<PartitionFieldMatch>>, IPartitionFieldCriteria
    {
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }
}