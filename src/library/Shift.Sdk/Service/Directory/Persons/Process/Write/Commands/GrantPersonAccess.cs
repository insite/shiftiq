using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.People.Write
{
    public class GrantPersonAccess : Command
    {
        public DateTimeOffset Granted { get; set; }
        public string GrantedBy { get; set; }

        public GrantPersonAccess(Guid personId, DateTimeOffset granted, string grantedBy)
        {
            AggregateIdentifier = personId;
            Granted = granted;
            GrantedBy = grantedBy;
        }
    }
}
