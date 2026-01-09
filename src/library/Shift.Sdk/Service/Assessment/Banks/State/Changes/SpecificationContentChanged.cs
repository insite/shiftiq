using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;
namespace InSite.Domain.Banks
{
    public class SpecificationContentChanged : Change
    {
        public Guid Specification { get; set; }
        public ContentExamSpecification Content { get; set; }

        public SpecificationContentChanged(Guid spec, ContentExamSpecification content)
        {
            Specification = spec;
            Content = content;
        }
    }
}
