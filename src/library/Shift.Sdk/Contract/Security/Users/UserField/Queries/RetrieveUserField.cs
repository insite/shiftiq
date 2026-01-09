using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveUserField : Query<UserFieldModel>
    {
        public Guid SettingIdentifier { get; set; }
    }
}