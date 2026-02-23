using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertUserField : Query<bool>
    {
        public Guid SettingId { get; set; }
    }
}