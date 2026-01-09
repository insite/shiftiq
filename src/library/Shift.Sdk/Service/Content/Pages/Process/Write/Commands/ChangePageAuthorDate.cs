using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageAuthorDate : Command
    {
        public DateTimeOffset? AuthorDate { get; set; }
        public ChangePageAuthorDate(Guid page, DateTimeOffset? authorDate)
        {
            AggregateIdentifier = page;
            AuthorDate = authorDate;
        }
    }
}
