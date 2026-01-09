using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class AuthorDateChanged : Change
    {
        public DateTimeOffset? AuthorDate { get; set; }
        public AuthorDateChanged(DateTimeOffset? authorDate)
        {
            AuthorDate = authorDate;
        }
    }
}
