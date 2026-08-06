using System;

using Shift.Common;
using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeCriterionContent : Command
    {
        public Guid Criterion { get; set; }
        public ContentExamCriterion Content { get; set; }

        public ChangeCriterionContent(Guid bank, Guid criterion, ContentExamCriterion content)
        {
            AggregateIdentifier = bank;
            Criterion = criterion;
            Content = content;
        }
    }
}
