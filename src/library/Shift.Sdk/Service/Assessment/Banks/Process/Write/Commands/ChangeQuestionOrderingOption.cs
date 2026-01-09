using System;

using Shift.Common.Timeline.Commands;

using Shift.Common;
namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionOrderingOption : Command
    {
        public Guid Question { get; set; }
        public Guid Option { get; set; }
        public ContentTitle Content { get; set; }

        public ChangeQuestionOrderingOption(Guid bank, Guid question, Guid option, ContentTitle content)
        {
            AggregateIdentifier = bank;
            Question = question;
            Option = option;
            Content = content;
        }
    }
}
