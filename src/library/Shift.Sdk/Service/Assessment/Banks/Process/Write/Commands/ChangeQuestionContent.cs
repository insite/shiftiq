using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;
namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionContent : Command
    {
        public Guid Question { get; set; }
        public ContentExamQuestion Content { get; set; }

        public ChangeQuestionContent(Guid bank, Guid question, ContentExamQuestion content)
        {
            AggregateIdentifier = bank;
            Question = question;
            Content = content;
        }
    }
}
