using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveGroupField : Query<GroupFieldModel>
    {
        public Guid SettingId { get; set; }
    }
}