using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookUserNoted : Change
    {
        public GradebookUserNoted(Guid user, string note, DateTimeOffset? added)
        {
            User = user;
            Note = note;
            Added = added;
        }

        public Guid User { get; set; }
        public string Note { get; set; }
        public DateTimeOffset? Added { get; set; }
    }
}