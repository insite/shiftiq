using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class DeleteField : Command
    {
        public Guid Field { get; set; }
        public Guid Form { get; set; }
        public Guid Question { get; set; }

        public DeleteField(Guid bank, Guid field, Guid form, Guid question)
        {
            AggregateIdentifier = bank;
            Field = field;
            Form = form;
            Question = question;
        }
    }
}