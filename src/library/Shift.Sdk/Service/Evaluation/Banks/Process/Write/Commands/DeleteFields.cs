using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteFields : Command
    {
        public Guid Form { get; set; }
        public Guid Question { get; set; }

        public DeleteFields(Guid bank, Guid form, Guid question)
        {
            AggregateIdentifier = bank;
            Form = form;
            Question = question;
        }
    }
}