using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Contacts;

namespace InSite.Application.Users.Write
{
    public class ModifyUserFieldText : Command
    {
        public UserField UserField { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return base.ToString() + $", UserField = {UserField}";
        }

        public ModifyUserFieldText(Guid userId, UserField userField, string value)
        {
            AggregateIdentifier = userId;
            UserField = userField;
            Value = value;
        }
    }
}
