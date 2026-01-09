using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageAuthorName : Command
    {
        public string AuthorName { get; set; }
        public ChangePageAuthorName(Guid page, string authorName)
        {
            AggregateIdentifier = page;
            AuthorName = authorName;
        }
    }
}
