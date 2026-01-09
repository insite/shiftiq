using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;
namespace InSite.Application.Banks.Write
{
    public class ChangeBankContent : Command
    {
        public ContentExamBank Content { get; set; }

        public ChangeBankContent(Guid bank, ContentExamBank content)
        {
            AggregateIdentifier = bank;
            Content = content;
        }
    }
}