using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;
namespace InSite.Application.Banks.Write
{
    public class ChangeSpecificationContent : Command
    {
        public Guid Specification { get; set; }
        public ContentExamSpecification Content { get; set; }

        public ChangeSpecificationContent(Guid bank, Guid spec, ContentExamSpecification content)
        {
            AggregateIdentifier = bank;
            Specification = spec;
            Content = content;
        }
    }
}
