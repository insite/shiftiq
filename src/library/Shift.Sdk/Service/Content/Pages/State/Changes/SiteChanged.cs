using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class SiteChanged : Change
    {
        public Guid? Site { get; set; }
        public SiteChanged(Guid? site)
        {
            Site = site;
        }
    }
}
