using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class PageObjectModified : Change
    {
        public string Type { get; set; }
        public Guid? Object { get; set; }

        public PageObjectModified(string type, Guid? @object)
        {
            Type = type;
            Object = @object;
        }
    }
}
