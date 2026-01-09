using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Banks
{
    public class FieldsDeleted : Change
    {
        public Guid Form { get; set; }
        public Guid Question { get; set; }

        public FieldsDeleted(Guid form, Guid question)
        {
            Form = form;
            Question = question;
        }
    }
}
