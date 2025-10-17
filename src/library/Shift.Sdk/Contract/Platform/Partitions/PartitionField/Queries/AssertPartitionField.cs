using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertPartitionField : Query<bool>
    {
        public Guid SettingIdentifier { get; set; }
    }
}