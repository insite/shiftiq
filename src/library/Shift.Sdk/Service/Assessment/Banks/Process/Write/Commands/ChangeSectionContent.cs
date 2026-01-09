using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;
namespace InSite.Application.Banks.Write
{
    public class ChangeSectionContent : Command
    {
        public Guid Set { get; set; }
        public ContentExamSection Content { get; set; }

        public ChangeSectionContent(Guid bank, Guid set, ContentExamSection content)
        {
            AggregateIdentifier = bank;
            Set = set;
            Content = content;
        }
    }
}