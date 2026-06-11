using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Pages.Write
{
    public class ChangePageSlug : Command
    {
        public string Slug { get; set; }
        public ChangePageSlug(Guid page, string slug)
        {
            AggregateIdentifier = page;
            Slug = slug;
        }
    }
}
