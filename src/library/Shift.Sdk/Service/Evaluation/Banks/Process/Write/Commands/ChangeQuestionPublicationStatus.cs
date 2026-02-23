using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionPublicationStatus : Command
    {
        public Guid Question { get; set; }
        public PublicationStatus Status { get; set; }

        public ChangeQuestionPublicationStatus(Guid bank, Guid question, PublicationStatus status)
        {
            AggregateIdentifier = bank;
            Question = question;
            Status = status;
        }
    }
}
