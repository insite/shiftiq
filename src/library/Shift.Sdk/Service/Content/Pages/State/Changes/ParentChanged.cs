using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages 
{
    public class ParentChanged : Change
    {
        public Guid? Parent { get; set; }
        public ParentChanged(Guid? parent)
        {
            Parent = parent;
        }
    }
}
