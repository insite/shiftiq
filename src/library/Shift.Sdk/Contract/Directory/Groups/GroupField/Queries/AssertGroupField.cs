using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertGroupField : Query<bool>
    {
        public Guid SettingIdentifier { get; set; }
    }
}