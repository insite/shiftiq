using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Contacts;

namespace InSite.Application.Users.Write
{
    public class ModifyUserFieldInt : Command
    {
        public UserField UserField { get; set; }
        public int? Value { get; set; }

        public override string ToString()
        {
            return base.ToString() + $", UserField = {UserField}";
        }

        public ModifyUserFieldInt(Guid userId, UserField userField, int? value)
        {
            AggregateIdentifier = userId;
            UserField = userField;
            Value = value;
        }
    }
}
