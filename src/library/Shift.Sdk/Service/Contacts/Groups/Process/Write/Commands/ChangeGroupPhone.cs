using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Groups.Write
{
    public class ChangeGroupPhone : Command
    {
        public string Phone { get; }

        public ChangeGroupPhone(Guid group, string phone)
        {
            AggregateIdentifier = group;
            Phone = phone.NullIfEmpty();
        }
    }
}