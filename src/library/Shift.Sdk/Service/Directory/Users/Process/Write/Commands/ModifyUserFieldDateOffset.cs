using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Contacts;

namespace InSite.Application.Users.Write
{
    public class ModifyUserFieldDateOffset : Command
    {
        public UserField UserField { get; set; }
        public DateTimeOffset? Value { get; set; }

        public override string ToString()
        {
            return base.ToString() + $", UserField = {UserField}";
        }

        public ModifyUserFieldDateOffset(Guid userId, UserField userField, DateTimeOffset? value)
        {
            AggregateIdentifier = userId;
            UserField = userField;
            Value = value;
        }
    }
}
