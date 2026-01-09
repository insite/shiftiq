using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Banks.Write
{
    public class ChangeQuestionGradeItem2 : Command
    {
        public Guid Form { get; set; }
        public Guid Question { get; set; }
        public Guid? GradeItem { get; set; }

        public ChangeQuestionGradeItem2(Guid bank, Guid form, Guid question, Guid? gradeItem)
        {
            AggregateIdentifier = bank;
            Form = form;
            Question = question;
            GradeItem = gradeItem;
        }
    }
}
