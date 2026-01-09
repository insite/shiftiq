using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Banks
{
    public class SectionContentChanged : Change
    {
        public Guid Section { get; set; }
        public ContentExamSection Content { get; set; }

        public SectionContentChanged(Guid section, ContentExamSection content)
        {
            Section = section;
            Content = content;
        }
    }
}
