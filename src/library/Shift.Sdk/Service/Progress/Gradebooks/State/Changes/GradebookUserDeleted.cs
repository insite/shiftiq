using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookUserDeleted : Change
    {
        public GradebookUserDeleted(Guid user)
        {
            User = user;
        }

        public Guid User { get; set; }
    }
}