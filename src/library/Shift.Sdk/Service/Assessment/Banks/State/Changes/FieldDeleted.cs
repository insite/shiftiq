using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FieldDeleted : Change
    {
        public Guid Field { get; set; }
        public Guid Form { get; set; }
        public Guid Question { get; set; }

        public FieldDeleted(Guid field, Guid form, Guid question)
        {
            Field = field;
            Form = form;
            Question = question;
        }
    }
}
