using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Contacts;

namespace InSite.Application.Users.Write
{
    public class ModifyUserFieldBool : Command
    {
        public UserField UserField { get; set; }
        public bool? Value { get; set; }

        public override string ToString()
        {
            return base.ToString() + $", UserField = {UserField}";
        }

        public ModifyUserFieldBool(Guid userId, UserField userField, bool? value)
        {
            AggregateIdentifier = userId;
            UserField = userField;
            Value = value;
        }
    }
}
