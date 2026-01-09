using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrievePartitionField : Query<PartitionFieldModel>
    {
        public Guid SettingIdentifier { get; set; }
    }
}